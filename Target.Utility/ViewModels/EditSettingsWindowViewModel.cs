using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Target.Utility.Annotations;
using Target.Utility.Properties;

namespace Target.Utility.ViewModels
{
    public class EditSettingsWindowViewModel : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public EditSettingsWindowViewModel()
        {
            this.SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        #endregion

        #region Properties

        public int ResizeWidth
        {
            get { return Settings.Default.ResizeWidth; }
            set
            {
                Settings.Default.ResizeWidth = value;
                OnPropertyChanged(nameof(ResizeWidth));
            }
        }

        public int ResizeHeight
        {
            get { return Settings.Default.ResizeHeight; }
            set
            {
                Settings.Default.ResizeHeight = value;
                OnPropertyChanged(nameof(ResizeHeight));
            }
        }


        public int ResizeMultiple
        {
            get { return Settings.Default.ResizeMultiple; }
            set
            {
                Settings.Default.ResizeMultiple = value;
                OnPropertyChanged(nameof(ResizeMultiple));
            }
        }

        public double Tolerance
        {
            get { return Settings.Default.Tolerance; }
            set
            {
                Settings.Default.Tolerance = value;
                OnPropertyChanged(nameof(Tolerance));
            }
        }

        public bool ResizeImage
        {
            get { return Settings.Default.ResizeImage; }
            set
            {
                Settings.Default.ResizeImage = value;
                OnPropertyChanged(nameof(ResizeImage));
            }
        }

        public ICommand SaveSettingsCommand { get; set; }

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

        private void SaveSettings(object obj)
        {
            Settings.Default.Save();
            MessageBox.Show("Settings saved");
        }

        #endregion

        #endregion

    }
}
