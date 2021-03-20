using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Models;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.Dto.DbProblemModels;
using TelegramBot.Dto.Helper;

namespace TelegramBot.BLL.Interfaces
{
    public interface IDbProblemsService
    {
        Task<PagedModel<DbProblemsListBusinessModel>> GetDbProblemsAsync(int page);
        Task<Response> GetDbProblems(Update update);
        Task<Response> GetDbProblemsFirstPageAsync(Update update);
        Task<Response> GetDbProblemsPrevPageAsync(Update update, int currentPage);
        Task<Response> GetDbProblemsNextPageAsync(Update update, int currentPage, int maxPage);
        Task<Response> GetDbProblemsLastPageAsync(Update update, int lastPage);
        Task<Response> GetDbProblemByIdAsync(Update update, int problemId);
        Task<Response> PrepareSolveData(Update update, int problemId);
        Task<Response> ComputeSolutionAsync(Update update);
    }
}
