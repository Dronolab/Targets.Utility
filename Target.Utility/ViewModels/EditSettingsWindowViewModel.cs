using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Target.Utility.Annotations;
using Target.Utility.Properties;
using MessageBox = System.Windows.MessageBox;

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
            this.ChooseOutputFolderPathCommand = new RelayCommand(ChooseOutputFolderPath);
        }

        #endregion

        #region Properties

        public int ResizeWidth
        {
            get => Settings.Default.ResizeWidth;
            set
            {
                Settings.Default.ResizeWidth = value;
                OnPropertyChanged(nameof(ResizeWidth));
            }
        }

        public int ResizeHeight
        {
            get => Settings.Default.ResizeHeight;
            set
            {
                Settings.Default.ResizeHeight = value;
                OnPropertyChanged(nameof(ResizeHeight));
            }
        }


        public int ResizeMultiple
        {
            get => Settings.Default.ResizeMultiple;
            set
            {
                Settings.Default.ResizeMultiple = value;
                OnPropertyChanged(nameof(ResizeMultiple));
            }
        }

        public double Tolerance
        {
            get => Settings.Default.Tolerance;
            set
            {
                Settings.Default.Tolerance = value;
                OnPropertyChanged(nameof(Tolerance));
            }
        }

        public bool KeepExif
        {
            get => Settings.Default.KeepExif;
            set
            {
                Settings.Default.KeepExif = value;
                OnPropertyChanged(nameof(KeepExif));
            }
        }

        public bool KeepSelectionBetweenImage
        {
            get => Settings.Default.KeepSelectionBetweenImage;
            set
            {
                Settings.Default.KeepSelectionBetweenImage = value;
                OnPropertyChanged(nameof(KeepSelectionBetweenImage));
            }
        }

        public bool KeepXmp
        {
            get => Settings.Default.KeepXmp;
            set
            {
                Settings.Default.KeepXmp = value;
                OnPropertyChanged(nameof(KeepXmp));
            }
        }

        public string OutputFolderPath
        {
            get => Settings.Default.OutputFolderPath;
            set
            {
                Settings.Default.OutputFolderPath = value;
                OnPropertyChanged(nameof(OutputFolderPath));
            }
        }

        public string TargetSliceImagePrefix
        {
            get => Settings.Default.TargetSliceImagePrefix;
            set
            {
                Settings.Default.TargetSliceImagePrefix = value;
                OnPropertyChanged(nameof(TargetSliceImagePrefix));
            }
        }

        public ICommand SaveSettingsCommand { get; set; }

        public ICommand ChooseOutputFolderPathCommand { get; set; }

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

        private void SaveSettings(object obj)
        {
            Settings.Default.Save();
            MessageBox.Show("Settings saved");
        }

        private void ChooseOutputFolderPath(object obj)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.OutputFolderPath = fbd.SelectedPath;
            }
        }

        #endregion

        #endregion

    }
}
