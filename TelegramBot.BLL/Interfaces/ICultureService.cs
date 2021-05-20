using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.BLL.Interfaces
{
    public interface ICultureService
    {
        void SetCulture(Update update);
    }
}