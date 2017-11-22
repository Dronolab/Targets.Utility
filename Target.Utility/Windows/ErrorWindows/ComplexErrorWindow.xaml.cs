using System.Windows;
using Target.Utility.ViewModels;

namespace Target.Utility.Windows.ErrorWindows
{
    /// <summary>
    /// Interaction logic for ComplexErrorWindow.xaml
    /// </summary>
    public partial class ComplexErrorWindow : Window
    {
        public ComplexErrorWindow(ViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
