using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Target.Utility.Windows
{
    /// <summary>
    /// Interaction logic for ViewResizedImageWindow.xaml
    /// </summary>
    public partial class ViewResizedImageWindow : Window
    {
        public ViewResizedImageWindow(INotifyPropertyChanged viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
