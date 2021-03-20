using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Models.Generics;

namespace TelegramBot.BLL.Interfaces
{
    public interface IDbSubmissionsService
    {
        Task<Response> GetDbSubmissions(Update update);
        Task<Response> GetDbSubmissionsFirstPageAsync(Update update);
        Task<Response> GetDbSubmissionsPrevPageAsync(Update update, int page);
        Task<Response> GetDbSubmissionsNextPageAsync(Update update, int page, int maxPage);
        Task<Response> GetDbSubmissionsLastPageAsync(Update update, int page);
        Task<Response> GetDbSubmissionsByIdAsync(Update update, int submissionId);
    }
}
