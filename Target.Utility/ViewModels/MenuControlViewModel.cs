using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Target.Utility.Annotations;
using Target.Utility.Events;

namespace Target.Utility.ViewModels
{
    public class MenuControlViewModel : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;
        private string _imageSize;

        #endregion

        #region Constructors

        public MenuControlViewModel()
        {
            this.LoadImageCommand = new RelayCommand(LoadImage);
            this.ResizeBatchCommand = new RelayCommand(ResizeBatch);
            this.ViewResizedImageCommand = new RelayCommand(ViewResizedImage);
            this.Add32PxDicisionCommand = new RelayCommand(Add32PxDicision);
            this.RestoreImageCommand = new RelayCommand(RestoreImage);
            this.ResetTargetSelectionCommand = new RelayCommand(ResetTargetSelection);
            this.SliceCommand = new RelayCommand(Slice, CanSlice);
            this.EditSettingsCommand = new RelayCommand(EditSettings);
            Bootstrapper.GetEventAggregator().GetEvent<ImageSizeFoundEvent>().Subscribe(ImageLoaded);
        }

        #endregion

        #region Properties

        public ICommand LoadImageCommand { get; set; }
        public ICommand ResizeBatchCommand { get; set; }

        public ICommand Add32PxDicisionCommand { get; set; }

        public ICommand ResetTargetSelectionCommand { get; set; }

        public ICommand RestoreImageCommand { get; set; }

        public ICommand SliceCommand { get; set; }

        public ICommand ViewResizedImageCommand { get; set; }

        public ICommand EditSettingsCommand { get; set; }

        public string ImageSize
        {
            get => _imageSize;
            set
            {
                _imageSize = value;
                OnPropertyChanged(nameof(ImageSize));
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

        private bool CanSlice(object parameter)
        {
            // Validate if there is a picture loaded and that a selection has been made
            return true;
        }

        private void ResizeBatch(object obj)
        {
            ImageController.Instance.ResizeBatch();
        }

        private void Slice(object parameter)
        {
            Bootstrapper.GetEventAggregator().GetEvent<SliceImageEvent>().Publish();
        }

        private void LoadImage(object parameter)
        {
            ImageController.Instance.LoadImage();
        }
        private void RestoreImage(object parameter)
        {
            Bootstrapper.GetEventAggregator().GetEvent<RestoreImageEditionsEvent>().Publish();
        }

        private void ResetTargetSelection(object obj)
        {
            Bootstrapper.GetEventAggregator().GetEvent<ResetTargetSelectionEvent>().Publish();
        }

        private void Add32PxDicision(object obj)
        {
            Bootstrapper.GetEventAggregator().GetEvent<Add32PxGridEvent>().Publish();
        }

        private void ImageLoaded(ImageSizeFoundEvent obj)
        {
            this.ImageSize = obj.ImageSize;
        }

        private void ViewResizedImage(object obj)
        {
            Bootstrapper.ViewResizedImage();
        }

        private void EditSettings(object obj)
        {
            Bootstrapper.ShowEditSettingsView();
        }

        #endregion

        #endregion

    }
}
