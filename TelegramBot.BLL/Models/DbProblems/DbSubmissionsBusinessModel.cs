using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot.BLL.Helpers.Enums;

namespace TelegramBot.BLL.Models.DbProblems
{
    //Решения задачи

    public class DbSubmissionsBusinessModel
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string UserId { get; set; }
        public SubmissionsStatusesEnum Status { get; set; }
        public DateTime SubmitDateTime { get; set; }
        public DateTime? CheckDateTime { get; set; }
        public bool IsAccepted { get; set; }
        public string LogText { get; set; }
        public string SubmittedQuery { get; set; }
        public string UserResult { get; set; }
        public int JudgeTypeId { get; set; }
        public int LanguageId { get; set; }
    }
}
