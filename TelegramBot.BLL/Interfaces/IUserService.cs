using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Interfaces
{
    public interface IUserService
    {
        Task<Response> CreateUser(Update update);
        Task<Response> ValidateSolvewayCodeAsync(Update update);
        Task<Response> SetLanguage(Update update, LanguagesEnum lang);
    }
}