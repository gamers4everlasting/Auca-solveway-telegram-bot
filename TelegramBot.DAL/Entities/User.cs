using System;
using TelegramBot.DAL.Enums;

namespace TelegramBot.DAL.Entities
{
    public class User : EntityBase<int>
    {
        public int? SolvewayUserId { get; set; }
        public int TelegramUserId { get; set; }
        public string StudyCode { get; set; }
        public string StudyBearer { get; set; }
        public DateTime? BearerExpiresIn { get; set; }
        public ClientStateEnum State { get; set; }
        public int ProblemId { get; set; }
    }
}
