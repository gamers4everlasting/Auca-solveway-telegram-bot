using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.BLL.Services
{
    public interface ITelegramBotService
    {
        Task GetUpdates(Update update);
    }
}