using TelegramBot.DAL.Enums;

namespace TelegramBot.DAL.Entities
{
    public class UserState : EntityBase<int>
    {
        public int UserId { get; set; }
        public ClientStateEnum State { get; set; }
    }
}
