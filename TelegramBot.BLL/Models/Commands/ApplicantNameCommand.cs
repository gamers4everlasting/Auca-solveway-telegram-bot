using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.BLL.Models.Commands
{
    public class ApplicantNameCommand : Command
    {
        public override string Name => "/name"; //TODO COMMAND ENUM may be then table

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            //TODO: Command logic -_-
            await client.SendTextMessageAsync(chatId, "Введите ваше имя:", replyToMessageId: messageId);
        }

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.TextMessage)
                return false;

            return message.Text.Contains(Name);
        }
    }
}