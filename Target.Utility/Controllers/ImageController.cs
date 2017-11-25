using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Target.Utility.Models;
using Target.Utility.Properties;

namespace Target.Utility.Controllers
{
    public class ImageController
    {

        #region Fields

        private readonly List<TargetSelectionRegion> _selections;
        private readonly Image _img;

        #endregion

        #region Constructors

        public ImageController(Image img, List<TargetSelectionRegion> selections, string imgName)
        {
            this._selections = selections;
            this._img = img;
            this.ImageFileName = imgName;
        }

        #endregion

        #region Properties

        public string ImageFileName { get; }

        #endregion

        #region Methods

        #region Public

        public static Image ResizeImage(Image img)
        {
            var width = Settings.Default.ResizeWidth;
            var height = Settings.Default.ResizeHeight;
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

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
                    graphics.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrapMode);
                    wrapMode.Dispose();
                }
                graphics.Dispose();
            }

            return destImage;
        }

        /// <summary>
        /// Starts slicing the image.
        /// </summary>
        /// <returns></returns>
        public bool StartSlicing()
        {
            this.SliceImage();
            this._img.Dispose();
            return true;
        }

        #endregion

        #region Private

        /// <summary>
        /// Slices the image into pieces of m*m where m is the settings ResizeMultiple
        /// </summary>
        private void SliceImage()
        {
            var outputPath = Settings.Default.OutputFolderPath;
            var slicesPath = Path.Combine(outputPath, $"{this.ImageFileName} - {DateTime.Now:yyyy-MM-dd HH-mm-ss} - slices");
            Directory.CreateDirectory(slicesPath);

            this.AdjustSelection();

            // Start slicing
            this.CreateMiniatures(slicesPath); // needs the user selection
        }

        /// <summary>
        /// Adjusts the selection to prevent false positive.
        /// </summary>
        private void AdjustSelection()
        {
            var m = Settings.Default.ResizeMultiple;
            var tolerancePercent = Settings.Default.Tolerance;
            var t = m * tolerancePercent;

            foreach (var selection in this._selections)
            {
                // start point
                // check x offset
                var xStartOffset = selection.StartPixel.X % m;
                if (xStartOffset <= t)
                {
                    selection.StartPixel.X -= xStartOffset;
                }
                else if (m - xStartOffset <= t)
                {
                    selection.StartPixel.X += m - xStartOffset;
                }

                // check y offset
                var yStartOffset = selection.StartPixel.Y % m;
                if (yStartOffset <= t)
                {
                    selection.StartPixel.Y -= yStartOffset;
                }
                else if (m - yStartOffset <= t)
                {
                    selection.StartPixel.Y += m - yStartOffset;
                }

                // end point
                // check x offset
                var xEndOffset = selection.EndPixel.X % m;
                if (xEndOffset <= t)
                {
                    selection.EndPixel.X -= xEndOffset;
                }
                else if (m - xEndOffset <= t)
                {
                    selection.EndPixel.X += m - xEndOffset;
                }

                // check y offset
                var yEndOffset = selection.EndPixel.Y % m;
                if (yEndOffset <= t)
                {
                    selection.EndPixel.Y -= yEndOffset;
                }
                else if (m - yEndOffset <= t)
                {
                    selection.EndPixel.Y += m - yEndOffset;
                }

            }
        }

        /// <summary>
        /// Creates the miniatures.
        /// </summary>
        /// <param name="sliceDirectoryPath">The slice directory path.</param>
        private void CreateMiniatures(string sliceDirectoryPath)
        {
            var multiple = Settings.Default.ResizeMultiple;
            var width = Settings.Default.ResizeWidth;
            var height = Settings.Default.ResizeHeight;

            for (int i = 0; i < width / multiple; i++)
            {
                for (int j = 0; j < height / multiple; j++)
                {
                    var img = new Bitmap(multiple, multiple);
                    using (var graphics = Graphics.FromImage(img))
                    {
                        graphics.DrawImage(this._img, new Rectangle(0, 0, multiple, multiple), new Rectangle(i * multiple, j * multiple, multiple, multiple), GraphicsUnit.Pixel);

                        var hasTarget = false;
                        foreach (var selection in this._selections)
                        {
                            if (IsInRange(i * multiple, j * multiple, selection))
                            {
                                hasTarget = true;
                            }
                        }

                        var name = hasTarget ? $"{Settings.Default.TargetSliceImagePrefix}{Guid.NewGuid()} - i={i * multiple}, j={j * multiple}.jpg" : $"{Guid.NewGuid()} - i={i * multiple}, j={j * multiple}.jpg";
                        img.Save(Path.Combine(sliceDirectoryPath, name));
                        img.Dispose();
                        graphics.Dispose();
                    }
                }
            }

            // After the slices are done like a grid, we probably sliced a target in 1-2 images (32*32). 
            // We'll make custom slice that should contains the entire target
            this.CreateMiniatureOfTarget(sliceDirectoryPath);
        }

        /// <summary>
        /// Creates the miniature of target.
        /// </summary>
        /// <param name="sliceDirectoryPath">The slice directory path.</param>
        private void CreateMiniatureOfTarget(string sliceDirectoryPath)
        {
            // Settings
            var m = Settings.Default.ResizeMultiple;
            var width = Settings.Default.ResizeWidth;
            var height = Settings.Default.ResizeHeight;

            foreach (var selection in this._selections)
            {
                for (int i = selection.StartPixel.X; i < selection.EndPixel.X; i += 32)
                {
                    for (int j = selection.StartPixel.Y; j < selection.EndPixel.Y; j += 32)
                    {
                        if (i + m > width || j + m > height)
                        {
                            continue;
                        }

                        var img = new Bitmap(m, m);
                        using (var graphics = Graphics.FromImage(img))
                        {
                            graphics.DrawImage(this._img, new Rectangle(0, 0, m, m), new Rectangle(i, j, m, m), GraphicsUnit.Pixel);

                            img.Save(Path.Combine(sliceDirectoryPath, $"{Settings.Default.TargetSliceImagePrefix}{Guid.NewGuid()} - i={i}, j={j}.jpg"));
                            img.Dispose();
                            graphics.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the current selection is in the range of the current image slice
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="j">The j.</param>
        /// <param name="selection">The selection.</param>
        /// <returns>
        ///   <c>true</c> if the selection is in the slice; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInRange(int i, int j, TargetSelectionRegion selection)
        {
            var sx = selection.StartPixel.X;
            var sy = selection.StartPixel.Y;
            var ex = selection.EndPixel.X;
            var ey = selection.EndPixel.Y;

            //settings
            var m = Settings.Default.ResizeMultiple;

            // https://stackoverflow.com/questions/13513932/algorithm-to-detect-overlapping-periods
            // Checking if the ranges are overlapping
            var inRange = i < ex && sx < i + m;
            var heightinRangeToo = j < ey && sy < j + m;
            if (!heightinRangeToo)
            {
                inRange = false;
            }

            return inRange;
        }

        #endregion

        #endregion

        #region NeedsRefactoring

        /// <summary>
        /// Refactor this!!
        /// </summary>
        public static void ResizeBatch()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Multiselect = true,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                var files = dlg.FileNames.ToList();

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var resizedPath = Path.Combine(desktopPath, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} - Resized");
                Directory.CreateDirectory(resizedPath);

                var counter = 0;
                foreach (var filePath in files)
                {
                    ResizeImageDep(Settings.Default.ResizeWidth, Settings.Default.ResizeHeight, filePath, Path.Combine(resizedPath, $"{counter++}.jpg"));
                }

                MessageBox.Show("Done");
            }
        }

        /// <summary>
        /// refactor this
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="imagePath">The image path.</param>
        /// <param name="fileName">Name of the file.</param>
        private static void ResizeImageDep(int width, int height, string imagePath, string fileName)
        {
            var imageToResize = Image.FromFile(imagePath);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

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

                    destImage.Save(fileName);
                    wrapMode.Dispose();
                }
                graphics.Dispose();
            }
            imageToResize.Dispose();
            destImage.Dispose();
        }

        #endregion

    }
}
