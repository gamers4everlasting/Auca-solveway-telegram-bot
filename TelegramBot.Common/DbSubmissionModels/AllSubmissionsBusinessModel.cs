using System;
using System.Collections.Generic;
using TelegramBot.Common.Helper;

namespace TelegramBot.Common.DbSubmissionModels
{
    public class AllSubmissionsBusinessModel
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime SubmitDateTime { get; set; }
        public DateTime? CheckDateTime { get; set; }
        public bool IsAccepted { get; set; }
        public string LogText { get; set; }
        public string SubmitedQuery { get; set; }
        public string UserName { get; set; }
        public string UsersFirstName { get; set; }
        public string UsersLastName { get; set; }
        public string UserPhoto { get; set; }
        public string GroupName { get; set; }
        public string SubmissionCode { get; set; }
        public string ProblemCode { get; set; }
        public string JudgeTypeName { get; set; }
        public string SubjectName { get; set; }
        public List<ResultTableStructure> UserResult { get; set; }
        public string Comment { get; set; }
    }
}
