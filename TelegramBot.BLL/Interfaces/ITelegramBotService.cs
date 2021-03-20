using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.BLL.Interfaces
{
    public interface ITelegramBotService
    {
        Task GetUpdates(Update update);
    }
}