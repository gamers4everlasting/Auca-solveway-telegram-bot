﻿using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.BLL.Models.Generics
{
    public class Response
    {
        public string Message { get; set; }
        public long ChatId { get; set; }
        public int ReplyToMessageId { get; set; }
        public InlineKeyboardMarkup InlineKeyboardMarkup { get; set; }
        public bool? UpdateMessage { get; set; } //null if nothing changed
        public int UpdatingMessageId { get; set; }
        public ParseMode ParseMode { get; set; }
    }
}
