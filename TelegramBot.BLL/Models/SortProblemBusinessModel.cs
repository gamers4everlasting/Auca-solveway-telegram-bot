using System.Collections.Generic;

namespace TelegramBot.BLL.Models
{
    public class SortProblemBusinessModel
    {
        public int LanguageId { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public List<int> subjectIds { get; set; }
        public List<int> Collections { get; set; }
        public bool ExcludeCollection { get; set; }
        public List<int> tagIds { get; set; }
        public List<int> levelIds { get; set; }
        public bool? Solved { get; set; }
        public bool? NotSolved { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public SortingModel Sorting { get; set; }
    }
}
