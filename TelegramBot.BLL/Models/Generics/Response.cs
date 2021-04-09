using System.IO;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Helpers.Enums;

namespace TelegramBot.BLL.Models.Generics
{
    public class Response
    {
        public string Message { get; set; }
        public long ChatId { get; set; }
        public int ReplyToMessageId { get; set; }
        public InlineKeyboardMarkup InlineKeyboardMarkup { get; set; }
        public ResponseTypeEnum? ResponseType { get; set; } //TODO: structure better, what uses what.
        public int UpdatingMessageId { get; set; }
        public ParseMode ParseMode { get; set; }
        public IReplyMarkup ReplyKeyboardMarkup { get; set; }
        public FileStream ImageStream { get; set; }
        public bool DisableWebPagePreview { get; set; } = true;

        public Response()
        {
            ImageStream = null;
            ResponseType = null;
            ReplyToMessageId = 0;
        }
        
    }
}
