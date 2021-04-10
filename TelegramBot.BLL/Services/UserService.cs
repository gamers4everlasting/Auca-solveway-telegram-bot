using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
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
                    State = ClientStateEnum.SolvewayCodeSet,
                    CreatedUtc = DateTime.Now,
                    TelegramUserId = update.Message.From.Id
                });
                await Context.SaveChangesAsync();

            }

            return new Response
            {
                Message = Resources.Welcome,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Markdown,
                ResponseType = ResponseTypeEnum.NewMessage
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
            //TODO: uncomment when endpoint is done in solveway.club;
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
    }
}