using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Interfaces
{
    public interface ILanguageService
    {
        Task<Response> GetLanguagesAsync(Update update);
        Task<Response> SetLanguage(Update update, LanguagesEnum lang);
    }
}