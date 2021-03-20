using System.Collections.Generic;

namespace TelegramBot.Dto.DbSubmissionModels
{
    public class SubmissionsListBusinessModel
    {
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public bool? Correct { get; set; }
        public bool? Mistake { get; set; }
        public bool? Send { get; set; }
        public string UserId { get; set; }
        public List<string> StudentId { get; set; }
        public string ProblemCode { get; set; }
        public List<int> GroupIdList { get; set; }
        public List<int> CollectionIdList { get; set; }
        public int LanguageId { get; set; }
        public string UserName { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
