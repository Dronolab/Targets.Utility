using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Target.Utility.Annotations;

namespace Target.Utility.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public Window Window { get; set; }

        #endregion

        #region Methods

        #region Protected

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion

    }
}
