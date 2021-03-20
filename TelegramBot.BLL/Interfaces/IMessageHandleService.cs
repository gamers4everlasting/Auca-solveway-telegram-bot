using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Models.Generics;

namespace TelegramBot.BLL.Interfaces
{
    public interface IMessageHandleService
    {
        Task<Response> Handle(Update update);
    }
}