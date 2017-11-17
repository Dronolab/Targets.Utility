using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Target.Utility.Windows;

namespace Target.Utility
{
    public static class UserController
    {
        public static void ShowSelectUserWindow()
        {
            var window = new UserSelectionWindow();
            window.Show();
        }
    }
}
