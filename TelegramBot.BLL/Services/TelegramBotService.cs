using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Entities;
using TelegramBot.DAL.Enums;
using System;
using TelegramBot.BLL.Enums;
using TelegramBot.BLL.Helpers;

namespace TelegramBot.BLL.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ICommandService _commandService;
        private readonly IDbProblemsService _dbProblemsService;
        private readonly ApplicationContext _context;

        public TelegramBotService(ITelegramBotClient telegramBotClient, ApplicationContext context, IDbProblemsService dbProblemsService)
        {
            _telegramBotClient = telegramBotClient;
            _context = context;
            _dbProblemsService = dbProblemsService;
        }

        public async Task GetUpdates(Update update)
        {
            if (update == null) return;
            var response = update.Type switch
            {
                UpdateType.CallbackQueryUpdate => await HandleNewCallbackQueryAsync(update),
                UpdateType.MessageUpdate => await HandleNewMessageAsync(update),
                _ => new Response()
            };

            //var keyboard = new ReplyKeyboardMarkup
                //{
                //    Keyboard = new[]
                //    {
                //        new[]
                //        {
                //            new KeyboardButton("/Problems")
                //        },
                //        new[]
                //        {
                //            new KeyboardButton("/Submissions")
                //        }
                //    }
                //};

                switch (response.UpdateMessage)
                {
                    case null:
                        return;
                    case true:
                        await _telegramBotClient.EditMessageReplyMarkupAsync(response.ChatId,
                            response.UpdatingMessageId,
                            response.InlineKeyboardMarkup);
                        break;
                    default:
                        await _telegramBotClient.SendTextMessageAsync(response.ChatId, response.Message,
                            replyToMessageId: response.ReplyToMessageId,
                            replyMarkup: response.InlineKeyboardMarkup,
                            disableWebPagePreview: true,
                            parseMode: response.ParseMode);
                        break;
                }
        }

        private async Task<Response> HandleNewMessageAsync(Update update)
        {
            //if user is first time logged in then //await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Здравствуйте, Вас приветсвует Solveway bot", ParseMode.Html, false, false,0, keyboard);
            // and authenticate him, create a json file and place data in it.
            return (update.Message.Text) switch
            {
                "/Problems" => await _dbProblemsService.GetDbProblems(update),

                _ => await _dbProblemsService.ComputeSolution(update)
            };
        }

        private async Task<Response> HandleNewCallbackQueryAsync(Update update)
        {
            var command = update.CallbackQuery.Data.Split(' ');
            return (command[0]) switch
            {
                nameof(PaginationEnums.One) => await _dbProblemsService.GetDbProblemsFirstPageAsync(update),
                nameof(PaginationEnums.PrevPage) => await _dbProblemsService.GetDbProblemsPrevPageAsync(update, int.Parse(command[1])),
                nameof(PaginationEnums.NextPage) => await _dbProblemsService.GetDbProblemsNextPageAsync(update, int.Parse(command[1]), int.Parse(command[2])),
                nameof(PaginationEnums.LastPage) => await _dbProblemsService.GetDbProblemsLastPageAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.DbProblems) => await _dbProblemsService.GetDbProblemByIdAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.DbProblemSolve) => await _dbProblemsService.PrepareSolveData(update, int.Parse(command[1])),
                _ => new Response {UpdateMessage = null},
            };
        }

        
    }
}