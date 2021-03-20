using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.BLL.Models.Commands
{
    public class ClientNameCommand : Command
    {
        public override string Name => "/start"; //TODO COMMAND ENUM may be then table

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            var keyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new[] //TODO: продумать вариант если компании нет и единичный выбор и получать только обращения
         {
                new[]
                {
                    new KeyboardButton("/company Империя Пиццы"),
                     new KeyboardButton("/company Кайнар Групп"),
                      new KeyboardButton("/company Нават"),
                },
                  new[]
                {
                    new KeyboardButton("/company BaseApp"),
                     new KeyboardButton("/company Бюро"),
                      new KeyboardButton("/company 123"),
                },
                    new[]
                {
                    new KeyboardButton("/company BaseApp"),
                     new KeyboardButton("/company test"),
                      new KeyboardButton("/company tmp"),
                } }
            };
            //TODO: Command logic -_-
            await client.SendTextMessageAsync(chatId, "Здравствуйте, Вас приветсвует служба поддержки компании TimelySoft,для начала работы выберите вашу компанию", ParseMode.Html, false, false, replyToMessageId: messageId, keyboard); //TODO выбор компании из предложенных
            if(message.Text == "Империя Пиццы" || message.Text == "Кайнар Групп" || message.Text == "Нават")
            await client.SendTextMessageAsync(chatId, "Вы выбрали компанию"+message.Text+ "Теперь укажите ваше имя командой /name", replyToMessageId: messageId);
        }
        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.TextMessage)
                return false;

            return message.Text.Contains(Name);
        }
    }
}
