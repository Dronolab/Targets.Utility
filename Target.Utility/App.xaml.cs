using System.Windows;
using System.Windows.Threading;
using Target.Utility.Core;

namespace Target.Utility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Bootstrapper.Init();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Bootstrapper.ErrorController.ShowExceptionDetailed(e.Exception);
        }
    }
}
