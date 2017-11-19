using System.Windows;
using Target.Utility.ViewModels;

namespace Target.Utility.Windows.ErrorWindows
{
    /// <summary>
    /// Interaction logic for SimpleErrorWindow.xaml
    /// </summary>
    public partial class SimpleErrorWindow : Window
    {
        public SimpleErrorWindow(ViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
