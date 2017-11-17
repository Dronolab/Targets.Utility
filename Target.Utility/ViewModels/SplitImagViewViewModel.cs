using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Target.Utility.Annotations;
using Target.Utility.Events;
using Target.Utility.Models;
using Target.Utility.Properties;
using Target.Utility.Windows;

namespace Target.Utility.ViewModels
{
    public class SplitImagViewViewModel : INotifyPropertyChanged
    {

        #region Fields

        private ImageSource _displayedImage;
        private ImageModel _currentImageModel;
        private List<ImageModel> _imageList;
        private MainWindow _window;
        private bool _done;

        #endregion

        #region Constructors

        public SplitImagViewViewModel(MainWindow window)
        {
            this._window = window;
            this._imageList = new List<ImageModel>();
            this.SliceCommand = new RelayCommand(Slice);
            this.RemoveLastSelectionCommand = new RelayCommand(RemoveLastSelection);
            Bootstrapper.GetEventAggregator().GetEvent<NewImageFilesLoadedEvent>().Subscribe(ImageFilesLoaded);
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        private string _progressMessage;

        public string ProgressMessage
        {
            get => _progressMessage;
            set
            {
                _progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }

        public ImageSource DisplayedImage
        {
            get => _displayedImage;
            set
            {
                _displayedImage = value;
                OnPropertyChanged(nameof(DisplayedImage));
            }
        }

        public int ImageHeight => Settings.Default.ResizeHeight;

        public int ImageWidth => Settings.Default.ResizeWidth;

        public ICommand SliceCommand { get; set; }

        public ICommand RemoveLastSelectionCommand { get; set; }

        #endregion

        #region Methods

        #region Protected

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Private

        private void ImageFilesLoaded(NewImageFilesLoadedEvent imageFiles)
        {
            // Save the list
            var imageFilesPath = imageFiles.ImageFilsePath;
            foreach (var imageFilePath in imageFilesPath)
            {
                this._imageList.Add(new ImageModel(imageFilePath));
            }

            // Load the first image
            this._currentImageModel = this._imageList.FirstOrDefault(i => !i.Loaded);

            // Display it
            this.DisplayedImage = this._currentImageModel?.ImageSource;
            this._done = false;
        }

        private void Slice(object obj)
        {
            if (_done)
            {
                return;
            }

            this.LauchWorker();

            // Remove selection
            if (!Settings.Default.KeepSelectionBetweenImage)
            {
                this._window.ResetTargetSelection();
            }

            // Mark as done?? 
            this._currentImageModel.Done = true;

            // load the next image to be sliced
            this._currentImageModel = this._imageList.FirstOrDefault(i => !i.Done);

            if (this._currentImageModel != null)
            {
                // Display it
                this.DisplayedImage = this._currentImageModel?.ImageSource;
            }
            else
            {
                this.SessionDone();
            }
        }

        private void LauchWorker()
        {
            var selections = this._window.GetSelections();

            // Create a controller to slice the image
            var imgName = this._currentImageModel.Path.Split('\\').LastOrDefault();
            var imgController = new ImageController(this._currentImageModel.Image, selections, imgName);

            // Create background task with progress
            var worker = new BackgroundWorker();
            worker.DoWork += DoSlincing;
            worker.RunWorkerCompleted += WorkedDone;
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync(imgController);
            this.ProgressMessage = $"{imgController.ImageFileName} starting...";
        }

        private void SessionDone()
        {
            this._done = true;
            var nbSliced = this._imageList.Count(i => i.Done);
            var nbSlice = Settings.Default.ResizeHeight / Settings.Default.ResizeMultiple * //todo this math is invalid
                          Settings.Default.ResizeWidth / Settings.Default.ResizeMultiple;
            //Todo display nb point gained?
            this.ProgressMessage = $"Nice job! Tu as slicer {nbSliced} images! Ça représente ~{nbSlice * nbSliced} slices pour notre AI!";
            MessageBox.Show($"Nice job! Tu as slicer {nbSliced} images! Ça représente ~{nbSlice * nbSliced} slices pour notre AI!");

            this.ResetSession();
        }

        private void ResetSession()
        {
            // todo display done image
            this.DisplayedImage = null;
            this._imageList = new List<ImageModel>();
            this.ProgressMessage = string.Empty;
            this._currentImageModel = null;
        }

        private void RemoveLastSelection(object obj)
        {
            this._window.RemoveLastSelection();
        }

        private void WorkedDone(object sender, RunWorkerCompletedEventArgs e)
        {
            // check error, check cancel, then use result
            if (e.Error != null)
            {
                // handle the error
            }
            else if (e.Cancelled)
            {
                // handle cancellation
            }
            else
            {
                var result = (WorkerReportViewModel)e.Result;
                this.ProgressMessage = $"{result.ImageFileName} sliced!";
            }
        }

        private void DoSlincing(object sender, DoWorkEventArgs e)
        {
            var imgController = (ImageController)e.Argument;
            e.Result = new WorkerReportViewModel { Success = imgController.StartSlicing(), ImageFileName = imgController.ImageFileName };
        }

        #endregion

        #endregion

    }
}
