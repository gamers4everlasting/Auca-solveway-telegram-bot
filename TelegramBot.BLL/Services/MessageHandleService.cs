using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Services
{
    public class MessageHandleService: IMessageHandleService
    {
        private readonly ApplicationContext _context;
        private readonly IUserService _userService;
        private readonly IDbProblemsService _dbProblemsService;
        public MessageHandleService(ApplicationContext context, IUserService userService, IDbProblemsService dbProblemsService)
        {
            _context = context;
            _userService = userService;
            _dbProblemsService = dbProblemsService;
        }
        public async Task<Response> Handle(Update update)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == update.Message.From.Id);
            if (user == null) throw new UnauthorizedAccessException();

            return user.State switch
            {
                ClientStateEnum.SolvewayCodeSet => await _userService.ValidateSolvewayCodeAsync(update),
                ClientStateEnum.ProblemSet => await _dbProblemsService.ComputeSolutionAsync(update),
                ClientStateEnum.None => new Response {ResponseType = null},
                _ => new Response {ResponseType = null}
            };
        }
    }
}