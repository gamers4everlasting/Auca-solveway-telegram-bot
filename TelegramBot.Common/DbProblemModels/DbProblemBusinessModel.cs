using System.Collections.Generic;

namespace TelegramBot.Dto.DbProblemModels
{
    public class DbProblemBusinessModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int? SubjectSecurityProfilesId { get; set; }
        public string ProblemText { get; set; }
        public int ProblemLevel { get; set; }
        public string Solution { get; set; }
        public bool IsSync { get; set; }
        public string AuthorId { get; set; }
        public string ProblemName { get; set; }
        public string ProblemCode { get; set; }
        public string Author { get; set; }
        public bool IsOrdered { get; set; }
        public string StopWords { get; set; }
        public bool IsCheckedColumnNames { get; set; }
        public int? InstitutionId { get; set; }
        public List<int> Tags { get; set; }
        public List<int> DbCollections { get; set; }
        public List<int> SimilarProblem { get; set; }
        public string AuthorFullName { get; set; }
        //public List<CollectionLookUpModel> DbCollectionLookUpModels { get; set; }
        //public List<DbProblemLookUpModel> SimilarProblemLookUpModels { get; set; }
        //public List<TagBusinessLookUpModel> TagBusinessLookUpModels { get; set; }
        //public DbSubjectLookBusinessModel DbSubjectLookBusinessModel { get; set; }
        //public DbSubjectSecurityProfileLookUpModel DbSubjectSecurityProfileLookUpModel { get; set; }
        public ProblemSubTypeLookUpModel ProblemSubTypeLookUpModel { get; set; }
        //public InstitutionsLookUpBusinessModel Institution { get; set; }
        public string Comment { get; set; }
        //does author allowed to show
        public bool IsActive { get; set; }
        public int LanguageId { get; set; }
        //is it published in problem's archive
        public bool IsPublished { get; set; }
        //is it valid to solve
        public bool IsValid { get; set; }
        public string ProblemLink { get; set; }
        public int? QuestId { get; set; }
        //for chat
        public int? LastSubmissionId { get; set; }

        public DbProblemBusinessModel()
        {
            Tags = new List<int>();
            DbCollections = new List<int>();
            SimilarProblem = new List<int>();
        }
    }
}
