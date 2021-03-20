using System.ComponentModel;

namespace TelegramBot.Dto.DbProblemModels
{
    //Модель для отображения данных на странице Задач у студента и преподавателя
    public class DbProblemsListBusinessModel
    {
        public int Id { get; set; }
        [DisplayName("База данных")]
        public string SubjectName { get; set; }
        [DisplayName("Уровень")]
        public int ProblemLevel { get; set; }
        [DisplayName("Название")]
        public string ProblemName { get; set; }

        public string ProblemCode { get; set; }
        //Percentage of users who solved the problem
        public double PercentOfUsersSolved { get; set; }

        public bool IsSolved { get; set; }

        //Average task rating among users
        public double Raiting { get; set; }

        //SubType of This Problem
        public ProblemSubTypeLookUpModel ProblemSubTypeLookUpModel { get; set; }
    }
}
