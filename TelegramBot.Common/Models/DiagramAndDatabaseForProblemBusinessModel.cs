using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.Common.Models
{
    public class DiagramAndDatabaseForProblemBusinessModel
    {
        public string SubjectName { get; set; }
        public string Description { get; set; }

        public List<FileBusinessModel> Diagrams { get; set; }

        //Will be implemented in future.
        //public List<DbSubjectDatabasesForProblemBusinessModel> DatabasesForProblem { get; set; }

        //public List<DbSubjectJudgeTypeForProblemBusinessModel> SubjectJudgeTypeForProblem { get; set; }

    }
}
