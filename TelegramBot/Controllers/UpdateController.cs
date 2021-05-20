using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.BLL.Interfaces;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICultureService _cultureService;

        public UpdateController(ITelegramBotService telegramBotService, ICultureService cultureService)
        {
            _telegramBotService = telegramBotService;
            _cultureService = cultureService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            _cultureService.SetCulture(update);
            
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

    }
}