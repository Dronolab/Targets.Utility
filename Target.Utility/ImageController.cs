using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Target.Utility.Events;
using Target.Utility.Properties;

namespace Target.Utility
{
    public class ImageController
    {

        #region Fields

        private static ImageController _instance;
        private string _imageFilePath;
        private static readonly object Padlock = new object();

        // Applicable settings. We have a settings class, but sometime we don't want to use them
        private int _applicableMultiple = -1;
        private double _applicableTolerance = -1;

        #endregion

        #region Constructors

        private ImageController()
        {
        }

        #endregion

        #region Properties

        public static ImageController Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new ImageController());
                }
            }
        }

        #endregion

        #region Methods

        #region Public

        public void LoadImage()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                var filePath = dlg.FileName;
                Bootstrapper.GetEventAggregator().GetEvent<NewImageLoadedEvent>().Publish(new NewImageLoadedEvent(filePath));
                this._imageFilePath = filePath;
            }
        }

        public void LoadImage(string filePath)
        {
            if (filePath != null)
            {
                Bootstrapper.GetEventAggregator().GetEvent<NewImageLoadedEvent>().Publish(new NewImageLoadedEvent(filePath));
                this._imageFilePath = filePath;
            }
        }

        public Image ResizeImage(int width, int height)
        {
            var imageToResize = Image.FromFile(this._imageFilePath);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            foreach (var propItem in imageToResize.PropertyItems)
            {
                destImage.SetPropertyItem(propItem);
            }

            destImage.SetResolution(imageToResize.HorizontalResolution, imageToResize.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(imageToResize, destRect, 0, 0, imageToResize.Width, imageToResize.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void SliceImage(TargetSelectionRegion selection)
        {
            this._applicableMultiple = Settings.Default.ResizeMultiple;
            this._applicableTolerance = Settings.Default.Tolerance;

            Image resizedImage = null;
            if (Settings.Default.ResizeImage)
            {
                resizedImage = this.ResizeImage(Settings.Default.ResizeWidth, Settings.Default.ResizeHeight);
                this.ResizeSelection(ref selection);
            }
            else
            {
                var m = Settings.Default.ResizeMultiple;
                var imageToResize = Image.FromFile(this._imageFilePath);
                resizedImage = this.ResizeImage((imageToResize.Width / m) * m, (imageToResize.Height / m) * m);
                this.ResizeSelection(ref selection);
            }
            //this.FlatternSelection(ref selection);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var slicesPath = Path.Combine(desktopPath, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} - slices");
            Directory.CreateDirectory(slicesPath);

            // Start slicing
            this.CreateMiniatures(resizedImage, slicesPath, selection); // needs the user selection
        }

        #endregion

        #region Protected
        #endregion

        #region Private

        private void ResizeSelection(ref TargetSelectionRegion selection)
        {
            // First we need to check how much the main image has been resized
            var imageToResize = Image.FromFile(this._imageFilePath);

            var width = Settings.Default.ResizeWidth;
            var height = Settings.Default.ResizeHeight;

            var widthProp = width / (double)imageToResize.Width;
            var heightProp = height / (double)imageToResize.Height;

            var startX = selection.StartPixel.X;
            var startY = selection.StartPixel.Y;
            var endX = selection.EndPixel.X;
            var endY = selection.EndPixel.Y;

            var newStartX = Math.Round(startX * widthProp);
            var newStartY = Math.Round(startY * heightProp);
            var newEndX = Math.Round(endX * widthProp);
            var newEndY = Math.Round(endY * heightProp);

            selection.StartPixel = new Point(Convert.ToInt32(newStartX), Convert.ToInt32(newStartY));
            selection.EndPixel = new Point(Convert.ToInt32(newEndX), Convert.ToInt32(newEndY));
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="tolerance"></param>
        /// <param name="multiple"></param>
        private void FlattenrStartPoint(ref TargetSelectionRegion selection, double tolerance, double multiple)
        {
            var distanteStartX = selection.StartPixel.X % multiple;
            var distanteStartY = selection.StartPixel.Y % multiple;

            var moveX = false;
            var moveY = false;

            if (DontRespectTolerance(distanteStartX, tolerance, multiple))
            {
                moveX = true;
            }
            if (DontRespectTolerance(distanteStartY, tolerance, multiple))
            {
                moveY = true;
            }

            if (moveX || moveY)
            {
                var newX = moveX ? selection.StartPixel.X - (distanteStartX <= tolerance ? distanteStartX : -(multiple - tolerance)) : selection.StartPixel.X;
                var newY = moveY ? selection.StartPixel.Y - (distanteStartY <= tolerance ? distanteStartY : -(multiple - tolerance)) : selection.StartPixel.Y;
                selection.StartPixel = new Point((int)newX, (int)newY);
            }
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="tolerance"></param>
        /// <param name="multiple"></param>
        private void FlatternEndPoint(ref TargetSelectionRegion selection, double tolerance, double multiple)
        {
            var targetWidth = selection.EndPixel.X - selection.StartPixel.X;
            var targetHeight = selection.EndPixel.Y - selection.StartPixel.Y;

            var removeXOverflow = false;
            var removeYOverflow = false;

            var overflowWidthTargetMultiple = targetWidth % multiple;
            if (DontRespectTolerance(overflowWidthTargetMultiple, tolerance, multiple))
            {
                removeXOverflow = true;
            }
            var overflowHeightTargetMultiple = targetHeight % multiple;
            if (DontRespectTolerance(overflowHeightTargetMultiple, tolerance, multiple))// not good, this is a range, not a check todo
            {
                removeYOverflow = true;
            }

            if (removeXOverflow || removeYOverflow)
            {
                var oldEnd = selection.EndPixel;
                var newX = removeXOverflow ? oldEnd.X - (overflowWidthTargetMultiple <= tolerance ? overflowWidthTargetMultiple : -(multiple - tolerance)) : oldEnd.X;
                var newY = removeYOverflow ? oldEnd.Y - (overflowHeightTargetMultiple <= tolerance ? overflowHeightTargetMultiple : -(multiple - tolerance)) : oldEnd.Y;
                selection.EndPixel = new Point((int)newX, (int)newY);
            }
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="selection"></param>
        private void FlatternSelection(ref TargetSelectionRegion selection)
        {
            var multiple = Settings.Default.ResizeMultiple;
            var tolerance = multiple * Settings.Default.Tolerance; // todo settings

            // Firstly, we are checking the first point. we want something that start near of a multiple of the settings
            this.FlattenrStartPoint(ref selection, tolerance, multiple);

            // Sencondly, checking the distance of the target. If we want something near a multiple of the settings
            this.FlatternEndPoint(ref selection, tolerance, multiple);

            // Finnaly, we must check if the selection has width or height >= multiple (otherwise there is no selection)
            // todo
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="tolerance"></param>
        /// <param name="multiple"></param>
        /// <returns></returns>
        private bool DontRespectTolerance(double distance, double tolerance, double multiple)
        {
            return distance <= multiple * tolerance || distance >= multiple - multiple * tolerance;
        }

        private void CreateMiniatures(Image image, string sliceDirectoryPath, TargetSelectionRegion selection)
        {
            var multiple = this._applicableMultiple;

            var width = Settings.Default.ResizeWidth;
            var height = Settings.Default.ResizeHeight;

            if (!Settings.Default.ResizeImage)
            {
                width = image.Width / multiple * multiple;
                height = image.Height / multiple * multiple;
            }

            var counter = 0;
            for (int i = 0; i < width / multiple; i++)
            {
                for (int j = 0; j < height / multiple; j++)
                {
                    var img = new Bitmap(multiple, multiple);
                    using (var graphics = Graphics.FromImage(img))
                    {
                        graphics.DrawImage(image, new Rectangle(0, 0, multiple, multiple), new Rectangle(i * multiple, j * multiple, multiple, multiple), GraphicsUnit.Pixel);

                        // If there is a target or a part of a target, save as uneven number. Otherwise save as even number
                        // todo
                        if (IsInRange(i * multiple, j * multiple, selection))
                        {
                            counter += 1;
                            img.Save(Path.Combine(sliceDirectoryPath, $"AAA - {counter}.jpg"));
                            counter += 1;
                        }
                        else
                        {
                            img.Save(Path.Combine(sliceDirectoryPath, $"{counter}.jpg"));
                            counter += 2;
                        }
                    }
                }
            }

            this.CreateMiniatureOfTarget(image, sliceDirectoryPath, selection);
        }

        private void CreateMiniatureOfTarget(Image image, string sliceDirectoryPath, TargetSelectionRegion selection)
        {
            // Settings
            var t = this._applicableTolerance;
            var m = this._applicableMultiple;

            var tw = selection.EndPixel.X - selection.StartPixel.X; // target width
            var th = selection.EndPixel.Y - selection.StartPixel.Y; // target height

            // Rework endpoint flages
            var reworkWith = false;
            var reworkHeight = false;

            var wd = tw % m; // Width overflow
            if (tw > m && this.DontRespectTolerance(wd, t, m))
            {
                reworkWith = true;
            }

            var hd = th % m; // Width overflow
            if (th > m && this.DontRespectTolerance(hd, t, m))
            {
                reworkHeight = true;
            }

            if (reworkWith || reworkHeight)
            {
                var oldPoint = selection.EndPixel;
                var newX = reworkWith ? wd + t * m >= m ? oldPoint.X + m : oldPoint.X - wd : oldPoint.X; // shorthen of enlarge selection
                var newY = reworkHeight ? hd + t * m >= m ? oldPoint.Y + m : oldPoint.Y - hd : oldPoint.Y; // shorthen of enlarge selection
                selection.EndPixel = new Point(newX, newY); // We want a selection that is a multiple of multiple
            }

            for (int i = selection.StartPixel.X; i < selection.EndPixel.X; i += 32)
            {
                for (int j = selection.StartPixel.Y; j < selection.EndPixel.Y; j += 32)
                {
                    var img = new Bitmap(m, m);
                    var counter = 10001;
                    using (var graphics = Graphics.FromImage(img))
                    {
                        graphics.DrawImage(image, new Rectangle(0, 0, m, m), new Rectangle(i, j, m, m), GraphicsUnit.Pixel);

                        img.Save(Path.Combine(sliceDirectoryPath, $"AAA - {counter}.jpg"));
                        counter += 2;
                    }
                }
            }
        }

        private bool IsInRange(int i, int j, TargetSelectionRegion selection)
        {
            var sx = selection.StartPixel.X;
            var sy = selection.StartPixel.Y;
            var ex = selection.EndPixel.X;
            var ey = selection.EndPixel.Y;

            //settings
            var m = this._applicableMultiple;

            //            [Check if start is there]   [check if end is there]
            var inRange = sx >= i && sx < i + m || ex > i && ex < i + 32;

            //   [Check if start is there]  [check if end is there]
            if (!(sy >= j && sy < j + m || ey > j && ey < j + 32))
            {
                inRange = false;
            }

            return inRange;
        }

        #endregion

        #endregion

    }
}
