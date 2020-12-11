using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.BLL.Models.Commands
{
    public class ProblemPhotoCommand:Command
    {
        public override string Name => "/photo"; //TODO COMMAND ENUM may be then table

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            //TODO: Command logic -_-

            await client.SendTextMessageAsync(chatId, "Добавьте фотографии которые помогут решить Вашу проблемму", replyToMessageId: messageId);
        }
        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.TextMessage)
                return false;

            return message.Text.Contains(Name);
        }
    }
}
