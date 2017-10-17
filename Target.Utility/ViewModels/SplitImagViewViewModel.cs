using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Target.Utility.Annotations;
using Target.Utility.Events;

namespace Target.Utility.ViewModels
{
    public class SplitImagViewViewModel : INotifyPropertyChanged
    {

        #region Fields

        private ImageSource _imageSource;
        private int _imageHeight;
        private int _imageWidth;

        #endregion

        #region Constructors

        public SplitImagViewViewModel()
        {
            Bootstrapper.GetEventAggregator().GetEvent<NewImageLoadedEvent>().Subscribe(ImageLoded);
        }

        #endregion

        #region Properties
        public ICommand LoadImageCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value; 
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public int ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }

        public int ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        #endregion

        #region Methods

        #region Public
        #endregion

        #region Protected

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private

        private void ImageLoded(NewImageLoadedEvent image)
        {
            this.ImageSource = image.LoadImage();
            var imageSize = image.GetImageSize();

            this.ImageHeight = imageSize.Height;
            this.ImageWidth = imageSize.Width;

            var sSize = $"{imageSize.Width}x{imageSize.Height}";

            Bootstrapper.GetEventAggregator().GetEvent<ImageSizeFoundEvent>().Publish(new ImageSizeFoundEvent(sSize));
        }

        #endregion

        #endregion

    }
}
