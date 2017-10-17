using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Events;

namespace Target.Utility.Events
{
    public class NewImageLoadedEvent : PubSubEvent<NewImageLoadedEvent>
    {

        #region Fields
        #endregion

        #region Constructors

        public NewImageLoadedEvent(string imageFilePath)
        {
            this.ImageFilePath = imageFilePath;
        }

        public NewImageLoadedEvent()
        {
            
        }

        #endregion

        #region Properties

        public string ImageFilePath { get; set; }

        #endregion

        #region Methods

        #region Public

        public ImageSource LoadImage()
        {
            return new BitmapImage(new Uri(this.ImageFilePath));
        }

        public Size GetImageSize()
        {
            var image = Image.FromFile(this.ImageFilePath);

            return new Size(image.Width, image.Height);
        }

        #endregion

        #region Protected
        #endregion

        #region Private
        #endregion

        #endregion

    }
}
