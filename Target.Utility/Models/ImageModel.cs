using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Target.Utility.Models
{
    public class ImageModel : IDisposable
    {
        private BitmapImage _imageSource;

        public ImageModel(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public bool Loaded { get; private set; }

        public bool Done { get; set; }

        public Image Image { get; private set; }

        public ImageSource ImageSource
        {
            get
            {
                if (!this.Loaded)
                {
                    // Load it
                    this.Image = Image.FromFile(Path);

                    // Resize it
                    this.Image = ImageController.ResizeImage(this.Image);

                    // Convert it to CDI
                    this._imageSource = new BitmapImage();

                    using (var memStream2 = new MemoryStream())
                    {
                        this.Image.Save(memStream2, ImageFormat.Jpeg);

                        this._imageSource.BeginInit();
                        this._imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        this._imageSource.UriSource = new Uri(Path);
                        this._imageSource.StreamSource = memStream2;
                        this._imageSource.EndInit();
                    }

                    // Mark it as loaded
                    this.Loaded = true;
                }

                return this._imageSource;
            }
        }

        public void Dispose()
        {
            Image?.Dispose();
        }
    }
}
