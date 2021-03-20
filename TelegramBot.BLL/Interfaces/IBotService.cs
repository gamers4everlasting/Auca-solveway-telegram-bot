using Telegram.Bot;

namespace TelegramBot.BLL.Interfaces
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
