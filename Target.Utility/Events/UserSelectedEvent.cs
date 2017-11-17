using Prism.Events;
using Target.Utility.Dtos;

namespace Target.Utility.Events
{
    public class UserSelectedEvent : PubSubEvent<UserSelectedEvent>
    {
        public SlackUserDto User { get; set; }

        public UserSelectedEvent(SlackUserDto user)
        {
            this.User = user;
        }

        public UserSelectedEvent()
        {
        }
    }
}
