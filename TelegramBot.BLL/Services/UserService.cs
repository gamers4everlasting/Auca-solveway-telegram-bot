using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
using LanguagesEnum = TelegramBot.DAL.Enums.LanguagesEnum;
using User = TelegramBot.DAL.Entities.User;

namespace TelegramBot.BLL.Services
{
    public class UserService: BaseAuth, IUserService
    {
        public UserService(ApplicationContext context, IHttpClientFactory factory) 
            : base(factory, context)
        {
        }
        
        /// <summary>
        /// First login to bot, create new user in db.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Response> CreateUser(Update update)
        {
            var user = Context.Users.FirstOrDefault(x => x.TelegramUserId == update.Message.From.Id);
            if (user?.StudyCode != null)
                return new Response
                {
                    ReplyKeyboardMarkup = KeyboardMarkupData.GeneralProblemsAndSubmissions,
                    Message = Resources.CurrentlyInSystem,
                    ChatId = update.Message.Chat.Id,
                    ParseMode = ParseMode.Markdown,
                    ResponseType = ResponseTypeEnum.NewMessage,
                };
            if (user == null)
            {
                await Context.Users.AddAsync(new User
                {
                    State = ClientStateEnum.LanguageCodeSet,
                    CreatedUtc = DateTime.Now,
                    TelegramUserId = update.Message.From.Id
                });
                await Context.SaveChangesAsync();
            }

            var preparedResponse = PrepareLanguageMessageResponse();
            return new Response
            {
                Message = preparedResponse.ResponseText,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Markdown,
                ResponseType = ResponseTypeEnum.NewMessage,
                InlineKeyboardMarkup = preparedResponse.InlineKeyboardMarkup
            };
        }

        private PreparedMessageContent PrepareLanguageMessageResponse()
        {
            var sb = new StringBuilder();
            sb.AppendLine(ResourcesTranslation.WelcomeEn)
                .AppendLine(ResourcesTranslation.WelcomeRu)
                .AppendLine(ResourcesTranslation.WelcomeKy);

            var buttons = new InlineKeyboardButton[1][];
            buttons[0] = new InlineKeyboardButton[3];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData(LanguagesEnum.En.ToString(), $"{LanguagesEnum.En}");
            buttons[0][1] = InlineKeyboardButton.WithCallbackData(LanguagesEnum.Ru.ToString(), $"{LanguagesEnum.Ru}");
            buttons[0][2] = InlineKeyboardButton.WithCallbackData(LanguagesEnum.Ky.ToString(), $"{LanguagesEnum.Ky}");
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
            
        }

        /// <summary>
        /// Not a callback, validates solveway club code and gets users token.
        /// not a callback method.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Response> ValidateSolvewayCodeAsync(Update update)
        {
            //TODO: uncomment when endpoint is created in solveway.club;
            //var response = await ValidateClientCode(update.Message.From.Id, update.Message.Text);

            var user = await Context.Users.FirstAsync(x => x.TelegramUserId == update.Message.From.Id);
            user.StudyCode = update.Message.Text;
            await Context.SaveChangesAsync();
            return new Response
            {
                Message = Resources.CodeValidated,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ResponseType = ResponseTypeEnum.NewMessage,
                ReplyKeyboardMarkup = KeyboardMarkupData.GeneralProblemsAndSubmissions
            };
        }

        /// <summary>
        /// Callback method. To set language
        /// </summary>
        /// <param name="update"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<Response> SetLanguage(Update update, LanguagesEnum lang)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == update.CallbackQuery.From.Id);
            user.Language = lang;
            Resources.Culture = new CultureInfo(lang.ToString());
            ErrorResources.Culture = new CultureInfo(lang.ToString());
            
            if (user.State == ClientStateEnum.LanguageCodeSet)
            {
                user.State = ClientStateEnum.SolvewayCodeSet;
                await Context.SaveChangesAsync();
                return new Response
                {
                    Message = Resources.Welcome,
                    ChatId = update.CallbackQuery.Message.Chat.Id,
                    ParseMode = ParseMode.Markdown,
                    ResponseType = ResponseTypeEnum.NewMessage
                };

            }
            
            await Context.SaveChangesAsync();
            
            return new Response
            {
                Message = Resources.LanguageChanged,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ResponseType = ResponseTypeEnum.NewMessage,
                ReplyKeyboardMarkup = KeyboardMarkupData.GeneralProblemsAndSubmissions
            };
        }
    }
}