using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly ITelegramBotService _telegramBotService;

        public UpdateController(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            setCulture(update);
            
            try
            {
                await _telegramBotService.GetUpdates(update);
            }
            catch (Exception e)
            {
                return Ok();
            }

            return Ok();
        }

        private void setCulture(Update update)
        {
            if (update.CallbackQuery != null)
            {
                var culture = update.CallbackQuery.From.LanguageCode;
                Resources.Culture = new CultureInfo(culture);
                ErrorResources.Culture = new CultureInfo(culture);
            }

            if (update.Message != null)
            {
                var culture = update.Message.From.LanguageCode;
                Resources.Culture = new CultureInfo(culture);
                ErrorResources.Culture = new CultureInfo(culture);
            }
        }
    }
}