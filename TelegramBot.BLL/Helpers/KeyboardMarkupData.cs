using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.BLL.Helpers
{
    public static class KeyboardMarkupData
    {
        public static readonly IReplyMarkup GeneralProblemsAndSubmissions = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton(MessageTextTypes.Problems)
                },
                new[]
                {
                    new KeyboardButton(MessageTextTypes.Submissions)
                }
            }
        };
        
        
    }
}