using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Generics;
using static TelegramBot.BLL.Helpers.MessageTextTypes;

namespace TelegramBot.BLL.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IDbProblemsService _dbProblemsService;
        private readonly IDbSubmissionsService _dbSubmissionsService;
        private readonly IUserService _userService;
        private readonly IMessageHandleService _messageHandleService;

        public TelegramBotService(ITelegramBotClient telegramBotClient, IDbProblemsService dbProblemsService, IDbSubmissionsService dbSubmissionsService, IUserService userService, IMessageHandleService messageHandleService)
        {
            _telegramBotClient = telegramBotClient;
            _dbProblemsService = dbProblemsService;
            _dbSubmissionsService = dbSubmissionsService;
            _userService = userService;
            _messageHandleService = messageHandleService;
        }

        public async Task GetUpdates(Update update)
        {
            if (update == null) return;
            try
            {
                var response = update.Type switch
                {
                    UpdateType.CallbackQueryUpdate => await HandleNewCallbackQueryAsync(update),
                    UpdateType.MessageUpdate => await HandleNewMessageAsync(update),
                    _ => new Response()
                };

                switch (response.ResponseType)
                {
                    case ResponseTypeEnum.Photo:
                        await _telegramBotClient.SendPhotoAsync(
                            response.ChatId,
                            new InputOnlineFile(response.ImageStream, "Image.png"));
                        return;
                    case ResponseTypeEnum.UpdateMessage:
                        await _telegramBotClient.EditMessageReplyMarkupAsync(response.ChatId,
                            response.UpdatingMessageId,
                            response.InlineKeyboardMarkup);
                        return;
                    case ResponseTypeEnum.NewMessage:
                        await _telegramBotClient.SendTextMessageAsync(response.ChatId,
                            response.Message,
                            replyToMessageId: response.ReplyToMessageId,
                            replyMarkup: response.InlineKeyboardMarkup ?? response.ReplyKeyboardMarkup,
                            disableWebPagePreview: response.DisableWebPagePreview,
                            parseMode: response.ParseMode);
                        return;
                    case null:
                        throw new Exception("Nothing changed");
                    default:
                        throw new Exception("Nothing changed");
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (update.CallbackQuery == null)
                {
                    await _telegramBotClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        ErrorResources.AuthorizationError
                    );
                }
                else //in case if user pushed a callback button.
                {
                    await _telegramBotClient.SendTextMessageAsync(
                        update.CallbackQuery.Message.Chat.Id,
                        ErrorResources.AuthorizationError
                    );
                }
            }

            
        }

        private async Task<Response> HandleNewMessageAsync(Update update)
        {
            return (update.Message.Text) switch
            {
                Start => await _userService.CreateUser(update),
                Problems => await _dbProblemsService.GetDbProblems(update),
                Submissions => await _dbSubmissionsService.GetDbSubmissions(update),
                _ => await _messageHandleService.Handle(update)
            };
        }

        private async Task<Response> HandleNewCallbackQueryAsync(Update update)
        {
            var command = update.CallbackQuery.Data.Split(' ');
            return (command[0]) switch
            {
                nameof(PaginationEnum.DbProblemsOne) => await _dbProblemsService.GetDbProblemsFirstPageAsync(update),
                nameof(PaginationEnum.DbProblemsPrevPage) => await _dbProblemsService.GetDbProblemsPrevPageAsync(update, int.Parse(command[1])),
                nameof(PaginationEnum.DbProblemsNextPage) => await _dbProblemsService.GetDbProblemsNextPageAsync(update, int.Parse(command[1]), int.Parse(command[2])),
                nameof(PaginationEnum.DbProblemsLastPage) => await _dbProblemsService.GetDbProblemsLastPageAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.DbProblems) => await _dbProblemsService.GetDbProblemByIdAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.DbProblemSolve) => await _dbProblemsService.PrepareSolveData(update, int.Parse(command[1])),
                nameof(PaginationEnum.DbSubmissionsOne) => await _dbSubmissionsService.GetDbSubmissionsFirstPageAsync(update),
                nameof(PaginationEnum.DbSubmissionsPrevPage) => await _dbSubmissionsService.GetDbSubmissionsPrevPageAsync(update, int.Parse(command[1])),
                nameof(PaginationEnum.DbSubmissionsNextPage) => await _dbSubmissionsService.GetDbSubmissionsNextPageAsync(update, int.Parse(command[1]), int.Parse(command[2])),
                nameof(PaginationEnum.DbSubmissionsLastPage) => await _dbSubmissionsService.GetDbSubmissionsLastPageAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.DbSubmissions) => await _dbSubmissionsService.GetDbSubmissionsByIdAsync(update, int.Parse(command[1])),
                nameof(SectionEnums.TableResult) => await _dbProblemsService.GetTableResultPictureAsync(update),
                _ => new Response()
            };
        }

        
    }
}