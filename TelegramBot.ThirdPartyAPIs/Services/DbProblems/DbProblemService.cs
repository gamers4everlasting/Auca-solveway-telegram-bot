using System;
using System.Threading.Tasks;
using TelegramBot.Dto.APIInterfaces;
using TelegramBot.Dto.DbProblemModels;

namespace TelegramBot.ThirdPartyAPIs.Services.DbProblems
{
    public class DbProblemService : IExternalDbProblemService
    {

        public DbProblemService(ApplicationContext context)
        {
            
        }
        public Task<DbProblemBusinessModel> GetById(int problemId)
        {
            throw new NotImplementedException();
        }
    }
}
