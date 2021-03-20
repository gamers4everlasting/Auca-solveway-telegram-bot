using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.BLL.Helpers
{
    public static class KeyboardMarkupData
    {
        public static IReplyMarkup GeneralProblemsAndSubmissions = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton("/Problems")
                },
                new[]
                {
                    new KeyboardButton("/Submissions")
                }
            }
        };
        
        
    }
}