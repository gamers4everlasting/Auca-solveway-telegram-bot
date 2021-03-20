using System;
using System.Collections.Generic;
using TelegramBot.Common.Helper;

namespace TelegramBot.Common.DbSubmissionModels
{
    public class DbSubmissionByIdBusinessModel
    {
        public string Solution { get; set; }

        public string LogText { get; set; }

        public int JudgeType { get; set; }

        public int ProblemId { get; set; }

        public string StudentUserName { get; set; }

        public DateTime? SubmitDateTime { get; set; }

        public bool HasAccessToProblem { get; set; }

        public List<ResultTableStructure> UserResult { get; set; }

        public DbSubmissionByIdBusinessModel()
        {
            HasAccessToProblem = true;
        }
    }
}
