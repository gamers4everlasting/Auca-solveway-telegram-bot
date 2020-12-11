using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Services;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Entities;
using TelegramBot.DAL.Enums;

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

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            //Parse the update.Message.Text and call the appropriate service.
           await _telegramBotService.GetUpdates(update);

            return Ok();
        }
    }
}