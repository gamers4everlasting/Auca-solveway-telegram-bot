using System;
using System.Collections.Generic;

namespace TelegramBot.Common.Helper
{
    [Serializable]
    public class DbValidationResult
    {
        public bool Result { get; set; }
        public List<string> Description { get; set; }
        public List<ResultTableStructure> UserResult { get; set; }
        public ResultTableStructure SolutionResult { get; set; }

        public DbValidationResult()
        {
            Description = new List<string>();
            UserResult = new List<ResultTableStructure>();
        }
    }
}
