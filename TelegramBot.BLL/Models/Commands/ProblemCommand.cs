using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.BLL.Models.Commands
{
    public class ProblemCommand:Command
    {
        public override string Name => "/problem"; //TODO COMMAND ENUM may be then table

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
           // var c = await Bot.Get();
            //TODO: Command logic -_-
            await client.SendTextMessageAsync(chatId, "Опишите вашу проблему", replyToMessageId: messageId);
          //  client.OnMessage += Client_OnMessage;
        }

        //private void Client_OnMessage(object sender, MessageEventArgs e)
        //{
        //     var c = await Bot.Get();
        //    c.SendTextMessageAsync(e.Message.Chat.Id,"fvfvfv",replyToMessageId:e.Message.MessageId);
        //}

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.TextMessage)
                return false;

            return message.Text.Contains(Name);
        }
    }
}
