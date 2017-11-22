using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Target.Utility.Annotations;
using Target.Utility.Core;
using Target.Utility.Events;

namespace Target.Utility.ViewModels
{
    public class MenuControlViewModel : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MenuControlViewModel()
        {
            this.LoadImageCommand = new RelayCommand(LoadImages);
            this.ResizeBatchCommand = new RelayCommand(ResizeBatch);
            this.Add32PxDicisionCommand = new RelayCommand(Add32PxDicision);
            this.Remove32PxDicisionCommand = new RelayCommand(Remove32PxDicision);
            this.EditSettingsCommand = new RelayCommand(EditSettings);
        }

        #endregion

        #region Properties

        public ICommand LoadImageCommand { get; set; }

        public ICommand ResizeBatchCommand { get; set; }

        public ICommand Add32PxDicisionCommand { get; set; }

        public ICommand Remove32PxDicisionCommand { get; set; }

        public ICommand EditSettingsCommand { get; set; }

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
            throw new NotImplementedException();
            //ImageController.Instance.ResizeBatch();
        }

        private void LoadImages(object parameter)
        {
            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Multiselect = true,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (fileDialog.ShowDialog() == true)
            {
                var files = fileDialog.FileNames.ToList();
                Bootstrapper.GetEventAggregator().GetEvent<NewImageFilesLoadedEvent>().Publish(new NewImageFilesLoadedEvent(files));
            }
        }

        private void Add32PxDicision(object obj)
        {
            Bootstrapper.GetEventAggregator().GetEvent<Add32PxGridEvent>().Publish();
        }

        private void Remove32PxDicision(object obj)
        {
            Bootstrapper.GetEventAggregator().GetEvent<Remove32PxGridEvent>().Publish();
        }

        private void EditSettings(object obj)
        {
            Bootstrapper.ShowEditSettingsView();
        }

        #endregion

        #endregion

    }
}
