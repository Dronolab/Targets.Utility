using Target.Utility.Windows;

namespace Target.Utility.Controllers
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
