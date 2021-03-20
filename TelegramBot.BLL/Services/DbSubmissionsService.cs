using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.Common.DbSubmissionModels;
using TelegramBot.Dto.DbProblemModels;
using TelegramBot.Dto.DbSubmissionModels;
using TelegramBot.Dto.Extensions;
using TelegramBot.Dto.Helper;

namespace TelegramBot.BLL.Services
{
    public class DbSubmissionsService : BaseClient, IDbSubmissionsService
    {
        public DbSubmissionsService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
        }
        public async Task<Response> GetDbSubmissions(Update update)
        {
            var problemList = await GetDbSubmissionsAsync(1);
            var messageContent = PrepareSubmissionsListContent(problemList, 1);
            return new Response
            {
                InlineKeyboardMarkup = messageContent.InlineKeyboardMarkup,
                Message = messageContent.ResponseText,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0
            };
        }

        

        public async Task<PagedModel<AllSubmissionsBusinessModel>> GetDbSubmissionsAsync(int page)
        {
            await AuthenticateAsync();

            var model = new SubmissionsListBusinessModel
            {
                Send = false,
                Correct = false,
                Mistake = false,
                FromDate = null,
                ToDate = null,
                UserId = null,
                StudentId = new List<string>(),
                GroupIdList = new List<int>(),
                ProblemCode = null,
                LanguageId = 2,
                CollectionIdList = new List<int>(),
                Page = page,
                PageSize = ConstData.TotalItemsAmountForList
            };


            var response = await Client.PostAsJsonAsync("api/DbSubmissionsListApi/StudentSubmissionsList", model);

            return await response.Content.ReadAsJsonPagedModelAsync<AllSubmissionsBusinessModel>();
        }

        

        public async Task<Response> GetDbSubmissionsFirstPageAsync(Update update)
        {
            var problemList = await GetDbSubmissionsAsync(1);
            var messageContent = PrepareSubmissionsListContent(problemList, 1);
            return new Response
            {
                InlineKeyboardMarkup = messageContent.InlineKeyboardMarkup,
                Message = messageContent.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.UpdateMessage,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbSubmissionsPrevPageAsync(Update update, int currentPage)
        {
            if (currentPage == 1)
            {
                return new Response{ResponseType = null};
            }
            var problemList = await GetDbSubmissionsAsync(currentPage - 1);
            var messageContent = PrepareSubmissionsListContent(problemList, currentPage);
            return new Response
            {
                InlineKeyboardMarkup = messageContent.InlineKeyboardMarkup,
                Message = messageContent.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.UpdateMessage,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbSubmissionsNextPageAsync(Update update, int currentPage, int maxPage)
        {
            if (currentPage == maxPage)
            {
                return new Response{ResponseType = null};
            }

            var problemList = await GetDbSubmissionsAsync(currentPage + 1);
            var messageContent = PrepareSubmissionsListContent(problemList, currentPage + 1);
            return new Response
            {
                InlineKeyboardMarkup = messageContent.InlineKeyboardMarkup,
                Message = messageContent.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.UpdateMessage,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbSubmissionsLastPageAsync(Update update, int lastPage)
        {
            var problemList = await GetDbSubmissionsAsync(lastPage);
            var messageContent = PrepareSubmissionsListContent(problemList, lastPage);
            return new Response
            {
                InlineKeyboardMarkup = messageContent.InlineKeyboardMarkup,
                Message = messageContent.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.UpdateMessage,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbSubmissionsByIdAsync(Update update, int submissionId)
        {
            await AuthenticateAsync();

            var submissionResponse = await Client.GetAsync($"/api/DbSubmissionsApi/GetSubmission?id={submissionId}&role=Student");
            var submissionData = await submissionResponse.Content.ReadAsJsonAsync<DbSubmissionByIdBusinessModel>();

            var problemResponse = await Client.GetAsync($"api/DbProblemApi/GetById?problemId={submissionData.ProblemId}&languageId=1");
            var problemData = await problemResponse.Content.ReadAsJsonAsync<DbProblemBusinessModel>();

            //create a view for one submission data
            var content = PrepareSingleSubmissionContent(submissionData, problemData);
            return new Response
            {
                InlineKeyboardMarkup = content.InlineKeyboardMarkup,
                Message = content.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0
            };
        }

        private PreparedMessageContent PrepareSubmissionsListContent(PagedModel<AllSubmissionsBusinessModel> submissionsList, int currentPage)
        {

            var submissions = submissionsList.Data.ToList();
            var sb = new StringBuilder();
            var buttons = new InlineKeyboardButton[submissions.Count + 1][];//+1 for pagination

            sb.AppendLine("Submissions");
            sb.AppendLine("[Solved] [Code]-[Name]");
            sb.AppendLine("___________________________________________");
            for (var i = 0; i < submissions.Count; i++)
            {
                var submission = submissions[i];
                buttons[i] = new InlineKeyboardButton[1];
                var isSolved = submission.IsAccepted ? "❌" : "✅";
                buttons[i][0] =
                    InlineKeyboardButton.WithCallbackData($"{isSolved}| {submission.ProblemCode} | L-{submission.ProblemName} | {submission.Status}",
                        $"{SectionEnums.DbSubmissions} {submission.Id}");
            }

            buttons = AddPagination(buttons, currentPage, Convert.ToInt32(Math.Ceiling((double)submissionsList.Total / (ConstData.TotalItemsAmountForList * 1.0))));

            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }
        private InlineKeyboardButton[][] AddPagination(InlineKeyboardButton[][] buttons, int currentPage, int lastPage)
        {
            var index = buttons.Length - 1;
            buttons[index] = new InlineKeyboardButton[5];
            buttons[index][0] = InlineKeyboardButton.WithCallbackData("1", nameof(PaginationEnum.DbSubmissionsOne));
            buttons[index][1] = InlineKeyboardButton.WithCallbackData("<", nameof(PaginationEnum.DbSubmissionsPrevPage) + $" {currentPage}");
            buttons[index][2] = InlineKeyboardButton.WithCallbackData($"{currentPage}", nameof(PaginationEnum.DbSubmissionsCurrentPage));
            buttons[index][3] = InlineKeyboardButton.WithCallbackData(">", nameof(PaginationEnum.DbSubmissionsNextPage) + $" {currentPage} {lastPage}");
            buttons[index][4] = InlineKeyboardButton.WithCallbackData($"{lastPage}", nameof(PaginationEnum.DbSubmissionsLastPage) + $" {lastPage}");

            return buttons;
        }

        private PreparedMessageContent PrepareSingleSubmissionContent(DbSubmissionByIdBusinessModel submission, DbProblemBusinessModel problemData)
        {
            var sb = new StringBuilder();
            var buttons = new InlineKeyboardButton[1][];
            sb.AppendLine($"{problemData.ProblemCode} | {problemData.ProblemName} ");
            sb.AppendLine("-----------Problem description------------");
            sb.AppendLine($"{problemData.ProblemText}");
            sb.AppendLine("-------------Solution---------------------");
            sb.AppendLine($"{submission.Solution}");
            sb.AppendLine("-------------LogText----------------------");
            sb.AppendLine($"{submission.LogText}");
            sb.AppendLine("-------------Info-------------------------");
            sb.AppendLine($"Server: {submission.JudgeType} | {problemData.SubjectName}");
            sb.AppendLine();
            buttons[0] = new InlineKeyboardButton[2];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData("Resolve Problem", $"{SectionEnums.DbProblemSolve} {submission.ProblemId}");
            buttons[0][1] = InlineKeyboardButton.WithCallbackData("Correct Result", $"{SectionEnums.DbProblemCorrectResult} {submission.ProblemId}");
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }
    }
}
