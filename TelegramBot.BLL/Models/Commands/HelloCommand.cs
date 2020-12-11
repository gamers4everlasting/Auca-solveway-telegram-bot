using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.BLL.Models.Commands
{
    public class HelloCommand : Command
    {
        public override string Name => "/hello"; //TODO COMMAND ENUM may be then table

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            //TODO: Command logic -_-
            await client.SendTextMessageAsync(chatId, "Здравствуйте, Вас приветсвует служба поддержки компании TimelySoft,для начала работы выберите вашу компанию командой /company", replyToMessageId: messageId); //TODO выбор компании из предложенных
        }
        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.TextMessage)
                return false;

            return message.Text.Contains(Name);
        }
    }
}