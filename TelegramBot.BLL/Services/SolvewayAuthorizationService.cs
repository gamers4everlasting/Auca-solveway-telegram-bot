using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Services
{
    public class SolvewayAuthorizationService: ISolvewayAuthorizationService
    {
        private readonly ApplicationContext _context;
        public SolvewayAuthorizationService(ApplicationContext context)
        {
            _context = context;
        }
        /// <summary>
        /// validates solveway club code and gets users token.
        /// not a callback method.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Response> ValidateSolvewayCodeAsync(Update update)
        {
            var message = await ProcessInput(update);
            return new Response
            {
                Message = message,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0,
                ReplyKeyboardMarkup = KeyboardMarkupData.GeneralProblemsAndSubmissions
            };
        }

        private async Task<string> ProcessInput(Update update)
        {
            var user = await _context.Users.FirstAsync(x => x.TelegramUserId == update.Message.From.Id);
            if (user.State != ClientStateEnum.SolvewayCodeSet) return "Code validation command is not set in db. (error occured)";
            
            //send code to solveway.club and get bearer, save to db.
            user.State = ClientStateEnum.None;
            await _context.SaveChangesAsync(); 
            return user.Language switch
            {
                LanguagesEnum.En => "code was successfully validated!",
                LanguagesEnum.Ru => "код успешно прошел валидацию!",
                _ => "Code validation command is not set in db. (error occured with language)"
            };
        }
    }
}