using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Target.Utility.Annotations;
using Target.Utility.Core;
using Target.Utility.Dtos;
using Target.Utility.Events;
using Target.Utility.Windows;

namespace Target.Utility.ViewModels
{
    public class UserSelectionWindowViewModel : INotifyPropertyChanged
    {
        private readonly UserSelectionWindow _window;
        public UserSelectionWindowViewModel(UserSelectionWindow window)
        {
            this._window = window;
            var helper = new RestHelper();
            this.SelectUserCommand = new RelayCommand(SelectUser);
            Users = helper.GetAsync().Result;
            SelectedUser = Users.First(u => u.RealName.ToLower().Contains("benoit"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<SlackUserDto> Users { get; }

        public SlackUserDto SelectedUser { get; set; }

        public ICommand SelectUserCommand { get; set; }

        private void SelectUser(object obj)
        {
            var user = (SlackUserDto)obj;
            Bootstrapper.GetEventAggregator().GetEvent<UserSelectedEvent>().Publish(new UserSelectedEvent(user));
            this._window.Close();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
