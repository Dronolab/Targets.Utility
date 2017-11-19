using Prism.Events;
using Target.Utility.ViewModels;
using Target.Utility.Windows;

namespace Target.Utility.Core
{
    public class Bootstrapper
    {

        #region Fields

        private static IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        private Bootstrapper()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Methods

        #region Public

        public static void Init()
        {
            _eventAggregator = new EventAggregator();
        }

        public static IEventAggregator GetEventAggregator()
        {
            return _eventAggregator;
        }

        public static void ShowEditSettingsView()
        {

            var viewModel = new EditSettingsWindowViewModel();
            var window = new EditSettingsWindow(viewModel);

            window.Show();
        }

        #endregion

        #region Protected
        #endregion

        #region Private

        #endregion

        #endregion

    }
}
