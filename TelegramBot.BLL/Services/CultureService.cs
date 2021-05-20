using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Services
{
    public class CultureService : ICultureService
    {
        private readonly ApplicationContext _context;

        public CultureService(ApplicationContext context)
        {
            _context = context;
        }
        public void SetCulture(Update update)
        {
          var messageUserId = update.CallbackQuery?.From.Id ?? update.Message.From.Id;
            var userLanguage = _context.Users.First(x => x.TelegramUserId == messageUserId).Language;

            var userCulture = userLanguage.GetDescription();
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(userCulture);
            Resources.Culture = new CultureInfo(userCulture);
            ErrorResources.Culture = new CultureInfo(userCulture);
        }
    }
}