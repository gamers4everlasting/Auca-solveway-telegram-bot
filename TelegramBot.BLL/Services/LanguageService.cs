using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
using User = TelegramBot.DAL.Entities.User;

namespace TelegramBot.BLL.Services
{
    public class LanguageService: ILanguageService
    {
        private readonly ApplicationContext _context;
        public LanguageService(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<Response> GetLanguagesAsync(Update update)
        {
            //if User is first time -> save data to db;
            var user = _context.Users.FirstOrDefault(x => x.TelegramUserId == update.Message.From.Id);
            if (user == null)
            {
                await _context.Users.AddAsync(new User
                {
                    Language = LanguagesEnum.En,
                    State = ClientStateEnum.LanguageSet,
                    CreatedUtc = DateTime.Now,
                    TelegramUserId = update.Message.From.Id
                });
                await _context.SaveChangesAsync();
            }
            
            var content = PreparedContent(user == null);
            return new Response
            {
                InlineKeyboardMarkup = content.InlineKeyboardMarkup,
                Message = content.ResponseText,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0
            };
        }

        /// <summary>
        /// Callback method to Set the picked language.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<Response> SetLanguage(Update update, LanguagesEnum lang)
        {
            var message = await ProcessUserData(update, lang);
            return new Response
            {
                Message = message,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0
            };
        }

        private async Task<string> ProcessUserData(Update update, LanguagesEnum lang)
        {
            var user = await _context.Users.FirstAsync(x => x.TelegramUserId == update.CallbackQuery.From.Id);
            if (user.State == ClientStateEnum.LanguageSet)
            {
                user.Language = lang;
                user.State = ClientStateEnum.SolvewayCodeSet;
                await _context.SaveChangesAsync();
                //TODO: some logic; send lang, receive string on language;
                switch (lang)
                {
                    case LanguagesEnum.En:
                        return "Welcome to Solveway judge! \n please enter the code from solveway.club website: ";
                    case LanguagesEnum.Ru:
                        return "Добро пожаловать в Solveway судью! \n пожалуйста введите код, высланный вам в сообщении от solveway.club:";
                }
            }

            switch (lang)
            {
                case LanguagesEnum.En:
                    return "System language is English now.";
                case LanguagesEnum.Ru:
                    return "Системный язык теперь Русский.";
            }

            return "Language was not changed (error occured)";

        }

        private PreparedMessageContent PreparedContent(bool isFirstTime)
        {
            var sb = new StringBuilder();
            if (isFirstTime)
            {
                sb.AppendLine("Welcome to Solveway sql judge. Please select a system language.");
                sb.AppendLine("");
                sb.AppendLine("Добро пожаловать в Solveway! Выберите пожалуйста язык системы.");
                sb.AppendLine("");
            }
            else
            {
                sb.AppendLine("Please select a system language.");
                sb.AppendLine("");
                sb.AppendLine("Выберите пожалуйста язык системы.");
                sb.AppendLine("");
            }

            var languagesLength = Enum.GetNames(typeof(LanguagesEnum)).Length;
            var buttons = new InlineKeyboardButton[1][];
            buttons[0] = new InlineKeyboardButton[languagesLength];
            for (var i = 0; i < languagesLength; i++)
            {
                var langName = Enum.GetName(typeof(LanguagesEnum), i + 1);
                var flag = langName == nameof(LanguagesEnum.En) ? "EN" : "RU";
                buttons[0][i] = InlineKeyboardButton.WithCallbackData($"{flag}", langName);
            }
            
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }
    }
}