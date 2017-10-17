using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Target.Utility.Annotations;
using Target.Utility.Properties;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Target.Utility.ViewModels
{
    public class ViewResizedImageWindowViewModel : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource _imageSource;

        #endregion

        #region Constructors

        public ViewResizedImageWindowViewModel(ImageSource image)
        {
            this.ImageSource = image;
            this.SaveResizedImageCommand = new RelayCommand(SaveResizedImage);
        }

        #endregion

        #region Properties

        public string Width => Settings.Default.ResizeWidth.ToString();

        public string Height => Settings.Default.ResizeHeight.ToString();

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public ICommand SaveResizedImageCommand { get; set; }

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

        private void SaveResizedImage(object parameter)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Choose location";
            sfd.DefaultExt = "jpg";
            sfd.Filter = "Image files (*.jpg, *.png) | *.jpg; *.png";
            sfd.CheckPathExists = true;

            var showDialog = sfd.ShowDialog();
            if (showDialog.HasValue && showDialog.Value)
            {
                var filePath = sfd.FileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapImage)this.ImageSource));
                    encoder.Save(fileStream);
                }
            }
        }

        #endregion

        #endregion
    }
}
