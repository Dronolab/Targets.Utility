using System.Windows;
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
    }
}
