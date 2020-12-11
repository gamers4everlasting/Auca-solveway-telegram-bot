using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.BLL.Models.Contents
{
    public class PreparedMessageContent
    {
        public string ResponseText { get; set; }
        public InlineKeyboardMarkup InlineKeyboardMarkup { get; set; }
    }
    
}
