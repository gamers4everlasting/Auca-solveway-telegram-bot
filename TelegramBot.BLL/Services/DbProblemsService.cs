using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
using TelegramBot.Common.Helper;
using TelegramBot.Common.Models;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
using TelegramBot.Dto.DbProblemModels;
using TelegramBot.Dto.DbSubmissionModels;
using TelegramBot.Dto.Extensions;
using TelegramBot.Dto.Helper;
using File = Telegram.Bot.Types.File;

namespace TelegramBot.BLL.Services
{
    public class DbProblemsService: BaseClient, IDbProblemsService
    {
        private readonly ApplicationContext _context;
        public DbProblemsService(ApplicationContext context, IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _context = context;
        }

        public async Task<PagedModel<DbProblemsListBusinessModel>> GetDbProblemsAsync(int page)
        {
            await AuthenticateAsync();

            var model = new SortProblemBusinessModel
            {
                LanguageId = 1,
                Page = page,
                PageSize = ConstData.TotalItemsAmountForList,
                subjectIds = new List<int>(),
                Collections = new List<int>(),
                ExcludeCollection = false,
                tagIds = new List<int>(),
                levelIds = new List<int>(),
                Solved = false,
                NotSolved = false,
                Role = "Student",
                UserId = null,
                Sorting = new SortingModel() { Direction = "desc", Field = "Date" }
            };
            
            var response = await Client.PostAsJsonAsync("/api/DbProblemApi/SortDbProblems", model);

            return await response.Content.ReadAsJsonPagedModelAsync<DbProblemsListBusinessModel>();
        }

        public async Task<Response> GetDbProblems(Update update)
        {
            var problemList = await GetDbProblemsAsync(1);
            var messageContent = PrepareProblemListContent(problemList, 1);
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
        public async Task<Response> GetDbProblemsFirstPageAsync(Update update)
        {
            var problemList = await GetDbProblemsAsync(1);
            var messageContent = PrepareProblemListContent(problemList, 1);
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

        public async Task<Response> GetDbProblemsPrevPageAsync(Update update, int currentPage)
        {
            if (currentPage == 1)
            {
                return new Response{ResponseType = null};
            }
            var problemList = await GetDbProblemsAsync(currentPage - 1);
            var messageContent = PrepareProblemListContent(problemList, currentPage);
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

        public async Task<Response> GetDbProblemsNextPageAsync(Update update, int currentPage, int maxPage)
        {
            if (currentPage == maxPage)
            {
                return new Response();
            }

            var problemList = await GetDbProblemsAsync(currentPage + 1);
            var messageContent = PrepareProblemListContent(problemList, currentPage+1);
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

        public async Task<Response> GetDbProblemsLastPageAsync(Update update, int lastPage)
        {
            var problemList = await GetDbProblemsAsync(lastPage);
            var messageContent = PrepareProblemListContent(problemList, lastPage);
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

        public async Task<Response> GetDbProblemByIdAsync(Update update, int problemId)
        {
            await AuthenticateAsync();
            
            var response = await Client.GetAsync($"/api/DbProblemApi/GetById?problemId={problemId}&languageId=1");
            
            var problemData =  await response.Content.ReadAsJsonAsync<DbProblemBusinessModel>();
            var content = PrepareSingleProblemContent(problemData);
            var diagramLink = await GetDiagramLink(problemData.SubjectId);
            //var imgStream =
            //    DbImageGenerator.GenerateStream(
            //        "Hello my friend\n _____________________\n\t|Data1| Data2| Data3|\n |Data4|Data5|Data6| \n Local");

            return new Response
            {
                InlineKeyboardMarkup = content.InlineKeyboardMarkup, 
                Message = $"{diagramLink} {content.ResponseText}",
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Markdown,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                UpdatingMessageId = 0,
                DisableWebPagePreview = false
            };
        }

        private async Task<string> GetDiagramLink(int subjectId)
        {
            var response = await 
                Client.GetAsync($"api/DbProblemApi/GetDiagramsAndDatabases?subjectId={subjectId}&languageId=2");
            if (!response.IsSuccessStatusCode) return string.Empty;
            var model = await response.Content.ReadAsJsonAsync<DiagramAndDatabaseForProblemBusinessModel>();
            var link = model.Diagrams.Select(x => x.Location).FirstOrDefault();
            if(string.IsNullOrEmpty(link)) return string.Empty;
            return $"[ ](https://test.solveway.club{link})";
        }

        public async Task<Response> PrepareSolveData(Update update, int problemId)
        {
            var user = await _context.Users.FirstAsync(x => x.TelegramUserId == update.CallbackQuery.From.Id);
            user.State = ClientStateEnum.ProblemSet;
            user.ProblemId = problemId;
            await _context.SaveChangesAsync();
            
            return new Response
            {
                Message = user.Language == LanguagesEnum.En ? "your solution: ": "ваше решение: ", //ToDo: Localization
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.UpdateMessage,
                UpdatingMessageId = 0
            };
        }

        public async Task<Response> ComputeSolutionAsync(Update update)
        {
            var user = await _context.Users.FirstAsync(x => x.TelegramUserId == update.Message.From.Id);
            
            var model = new DbSubmissionsBusinessModel
            {
                ProblemId = user.ProblemId,
                SubmittedQuery = update.Message.Text,
                JudgeTypeId = 1,
                LanguageId = 1
            };

            await AuthenticateAsync();
            var response = await Client.PostAsJsonAsync("/api/DbCheckSolution/Create", model);

            var responseData = await response.Content.ReadAsJsonAsync<DbValidationResult>();
            
            var preparedResponse = PrepareSubmissionStatus(responseData, user.ProblemId);
            

            //set problemId to 0
            user.State = ClientStateEnum.None;
            user.ProblemId = 0;
            await _context.SaveChangesAsync();
            
            return new Response
            {
                Message = preparedResponse.ResponseText,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                ResponseType = ResponseTypeEnum.NewMessage,
                InlineKeyboardMarkup = preparedResponse.InlineKeyboardMarkup
            };
        }

        private PreparedMessageContent PrepareSubmissionStatus(DbValidationResult responseData, int problemId)
        {
            var sb = new StringBuilder();
            if (responseData.Result)
            {
                sb.AppendLine("Query is correct");
                
            }
            else
            {
                for (var i = 0; i < responseData.Description.Count; i++)
                {
                    if (responseData.Description.Contains("SUBMIS0004ER"))
                    {
                        var parsed = responseData.Description[0].Split(',');
                        sb.AppendLine($"The number of rows returned is different on database №-{i + 1}. quantity: {parsed[1]}. expected: {parsed[2]}");
                    }
                    else
                    {
                        sb.AppendLine(responseData.Description[0]);
                    }
                }
            }

            var buttons = new InlineKeyboardButton[1][];
            buttons[0] = new InlineKeyboardButton[1];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData("solve again", $"{SectionEnums.DbProblemSolve} {problemId}");//TODo: Localization
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }

        private PreparedMessageContent PrepareSingleProblemContent(DbProblemBusinessModel problem)//TODo Localization
        {
            var sb = new StringBuilder();
            var buttons = new InlineKeyboardButton[2][];
            sb.AppendLine($"{problem.ProblemCode} | {problem.ProblemName}");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine($"{problem.ProblemText}");
            sb.AppendLine();
            buttons[0] = new InlineKeyboardButton[2];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData("Diagram", $"{SectionEnums.DbProblemDiagram} {problem.SubjectId}");
            buttons[0][1] = InlineKeyboardButton.WithCallbackData("Correct Result", $"{SectionEnums.DbProblemCorrectResult} {problem.Id}");
            buttons[1] = new InlineKeyboardButton[1];
            buttons[1][0] = InlineKeyboardButton.WithCallbackData("Solve Problem", $"{SectionEnums.DbProblemSolve} {problem.Id}");
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }

        private PreparedMessageContent PrepareProblemListContent(PagedModel<DbProblemsListBusinessModel> problemsPagedModel, int currentPage)//TODO: Localization
        {
            var problems = problemsPagedModel.Data.ToList();
            var sb = new StringBuilder();
            var buttons = new InlineKeyboardButton[problems.Count + 1][];//+1 for pagination

            sb.AppendLine("List of Tasks");
            sb.AppendLine("[Solved] [Code] [Level] [SubjectArea]-[Name]");
            sb.AppendLine("___________________________________________");
            for (var i = 0; i < problems.Count; i++)
            {
                var problem = problems[i];
                buttons[i] = new InlineKeyboardButton[1];
                var isSolved = problem.IsSolved ? "❌" : "✅";
                buttons[i][0] =
                    InlineKeyboardButton.WithCallbackData($"{isSolved}| {problem.ProblemCode} | L-{problem.ProblemLevel} | {problem.SubjectName.Substring(0, 7) + "..."} - {problem.ProblemName}",
                        $"{SectionEnums.DbProblems} {problem.Id}");
            }

            buttons = AddPagination(buttons, currentPage, Convert.ToInt32(Math.Ceiling((double)problemsPagedModel.Total / (ConstData.TotalItemsAmountForList * 1.0))));

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
            buttons[index][0] = InlineKeyboardButton.WithCallbackData("1", nameof(PaginationEnum.DbProblemsOne));
            buttons[index][1] = InlineKeyboardButton.WithCallbackData("<", nameof(PaginationEnum.DbProblemsPrevPage) + $" {currentPage}");
            buttons[index][2] = InlineKeyboardButton.WithCallbackData($"{currentPage}", nameof(PaginationEnum.DbProblemsCurrentPage));
            buttons[index][3] = InlineKeyboardButton.WithCallbackData(">", nameof(PaginationEnum.DbProblemsNextPage) + $" {currentPage} {lastPage}");
            buttons[index][4] = InlineKeyboardButton.WithCallbackData($"{lastPage}", nameof(PaginationEnum.DbProblemsLastPage) + $" {lastPage}");

            return buttons;
        }
    }
}
