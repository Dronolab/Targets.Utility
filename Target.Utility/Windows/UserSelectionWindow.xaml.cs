using System.Windows;
using Target.Utility.ViewModels;

namespace Target.Utility.Windows
{
    /// <summary>
    /// Interaction logic for UserSelectionWindow.xaml
    /// </summary>
    public partial class UserSelectionWindow : Window
    {
        public UserSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new UserSelectionWindowViewModel(this);
        }
    }
}
