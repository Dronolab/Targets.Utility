using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Target.Utility.Annotations;
using Target.Utility.Controllers;
using Target.Utility.Core;
using Target.Utility.Dtos;
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
        private readonly MainWindow _window;
        private bool _ready;
        private SlackUserDto _user;
        private string _progressMessage;
        private int _selectionSquareSize;

        #endregion

        #region Constructors

        public SplitImagViewViewModel(MainWindow window)
        {
            this._window = window;
            this._imageList = new List<ImageModel>();
            this.SliceCommand = new RelayCommand(Slice);
            this.RemoveLastSelectionCommand = new RelayCommand(RemoveLastSelection);
            this.RemoveSelectionsCommand = new RelayCommand(RemoveSelections);
            this.DisplayedImage = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Images/AI.jpg"));
            this.SelectionSquaredSize = Settings.Default.ResizeMultiple;
            Bootstrapper.GetEventAggregator().GetEvent<NewImageFilesLoadedEvent>().Subscribe(ImageFilesLoaded);
            Bootstrapper.GetEventAggregator().GetEvent<UserSelectedEvent>().Subscribe(UserSelected);
        }

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public int SelectionSquaredSize
        {
            get => _selectionSquareSize;
            set
            {
                _selectionSquareSize = value;
                OnPropertyChanged(nameof(SelectionSquaredSize));
            }
        }


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
        public SlackUserDto User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        public int ImageHeight => Settings.Default.ResizeHeight;

        public int ImageWidth => Settings.Default.ResizeWidth;

        public ICommand SliceCommand { get; set; }

        public ICommand RemoveLastSelectionCommand { get; set; }

        public ICommand RemoveSelectionsCommand { get; set; }

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
            this._ready = true;
        }

        private void Slice(object obj)
        {
            if (!this._ready)
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

        private async void LauchWorker()
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

            // Attention this code is not thread safe
            var helper = new RestHelper();
            this.User.NewPoints = 1;
            this.User.Points += 1;
            await helper.UpdateAsync(this._user);
            this.User.NewPoints = 0;
        }

        private void SessionDone()
        {
            this._ready = false;
            var nbSliced = this._imageList.Count(i => i.Done);
            var nbSlice = Settings.Default.ResizeHeight / Settings.Default.ResizeMultiple *
                          Settings.Default.ResizeWidth / Settings.Default.ResizeMultiple;

            this.ResetSession();

            //Todo display nb point gained?
            this.ProgressMessage = $"Nice job! Tu as slicer {nbSliced} images! Ça représente ~{nbSlice * nbSliced} slices pour notre AI! Tu as maintenant {this.User.Points} points!";
            MessageBox.Show($"Nice job! Tu as slicer {nbSliced} images! Ça représente ~{nbSlice * nbSliced} slices pour notre AI! Tu as maintenant {this.User.Points} points!");

        }

        private void ResetSession()
        {
            // todo display done image
            this.DisplayedImage = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Images/AI.jpg"));
            this._imageList = new List<ImageModel>();
            this.ProgressMessage = string.Empty;
            this._window.ResetTargetSelection();
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
                var result = (WorkerReport)e.Result;
                this.ProgressMessage = $"{result.ImageFileName} sliced!";
            }
        }

        private void DoSlincing(object sender, DoWorkEventArgs e)
        {
            var imgController = (ImageController)e.Argument;
            e.Result = new WorkerReport { Success = imgController.StartSlicing(), ImageFileName = imgController.ImageFileName };
        }

        private void UserSelected(UserSelectedEvent obj)
        {
            this.User = obj.User;
        }

        private void RemoveSelections(object obj)
        {
            this._window.ResetTargetSelection();
        }

        #endregion

        #endregion

    }
}
