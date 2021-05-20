using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.BLL.Helpers.Resources;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.Common.Extensions;
using TelegramBot.Common.Helper;
using TelegramBot.Common.Models;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
using TelegramBot.Dto.DbProblemModels;
using TelegramBot.Dto.DbSubmissionModels;
using TelegramBot.Dto.Helper;

namespace TelegramBot.BLL.Services
{
    public class DbProblemsService: BaseAuth, IDbProblemsService
    {
        public DbProblemsService(ApplicationContext context, IHttpClientFactory clientFactory) : base(clientFactory, context)
        {
        }

        public async Task<PagedModel<DbProblemsListBusinessModel>> GetDbProblemsAsync(int page, int telegramUserId)
        {
            await AuthenticateAsync(telegramUserId);

            var model = new SortProblemBusinessModel
            {
                LanguageId = CultureInfo.CurrentCulture.GetCurrentCultureId(),
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
                Sorting = new SortingModel() { Direction = "asc", Field = "Date" }
            };
            
            var response = await Client.PostAsJsonAsync("/api/DbProblemApi/SortDbProblems", model);

            return await response.Content.ReadAsJsonPagedModelAsync<DbProblemsListBusinessModel>();
        }

        public async Task<Response> GetDbProblems(Update update)
        {
            var problemList = await GetDbProblemsAsync(1, update.Message.From.Id);
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
            var problemList = await GetDbProblemsAsync(1, update.CallbackQuery.From.Id);
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

        /// <summary>
        /// Callback query gets data for previous page.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public async Task<Response> GetDbProblemsPrevPageAsync(Update update, int currentPage)
        {
            if (currentPage == 1)
            {
                return new Response{ResponseType = null};
            }
            var problemList = await GetDbProblemsAsync(currentPage - 1, update.CallbackQuery.From.Id);
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

        /// <summary>
        /// Callback query, gets next problem page
        /// </summary>
        /// <param name="update"></param>
        /// <param name="currentPage"></param>
        /// <param name="maxPage"></param>
        /// <returns></returns>
        public async Task<Response> GetDbProblemsNextPageAsync(Update update, int currentPage, int maxPage)
        {
            if (currentPage == maxPage)
            {
                return new Response();
            }

            var problemList = await GetDbProblemsAsync(currentPage + 1, update.CallbackQuery.From.Id);
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

        /// <summary>
        /// Callback query, gets last data page
        /// </summary>
        /// <param name="update"></param>
        /// <param name="lastPage"></param>
        /// <returns></returns>
        public async Task<Response> GetDbProblemsLastPageAsync(Update update, int lastPage)
        {
            var problemList = await GetDbProblemsAsync(lastPage, update.CallbackQuery.From.Id);
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

        /// <summary>
        /// Callback query, gets Problem by Id
        /// </summary>
        /// <param name="update"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public async Task<Response> GetDbProblemByIdAsync(Update update, int problemId)
        {
            await AuthenticateAsync(update.CallbackQuery.From.Id);
            
            var response = await Client.GetAsync($"/api/DbProblemApi/GetById?problemId={problemId}&languageId={CultureInfo.CurrentCulture.GetCurrentCultureId()}");
            
            var problemData =  await response.Content.ReadAsJsonAsync<DbProblemBusinessModel>();
            var content = PrepareSingleProblemContent(problemData);
            var diagramLink = await GetDiagramLink(problemData.SubjectId);
            
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
                Client.GetAsync($"api/DbProblemApi/GetDiagramsAndDatabases?subjectId={subjectId}&languageId={CultureInfo.CurrentCulture.GetCurrentCultureId()}");
            if (!response.IsSuccessStatusCode) return string.Empty;
            var model = await response.Content.ReadAsJsonAsync<DiagramAndDatabaseForProblemBusinessModel>();
            var link = model.Diagrams.Select(x => x.Location).FirstOrDefault();
            if(string.IsNullOrEmpty(link)) return string.Empty;
            return $"[ ](https://test.solveway.club{link})";
        }

        /// <summary>
        /// Not a callback
        /// </summary>
        /// <param name="update"></param>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public async Task<Response> PrepareSolveData(Update update, int problemId)
        {
            var user = await Context.Users.FirstAsync(x => x.TelegramUserId == update.CallbackQuery.From.Id);
            user.State = ClientStateEnum.ProblemSet;
            user.ProblemId = problemId;
            await Context.SaveChangesAsync();
            return new Response
            {
                Message = Resources.EnterSolution,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ResponseType = ResponseTypeEnum.NewMessage,
            };
        }

        /// <summary>
        /// Not a callback, computes problem submission
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Response> ComputeSolutionAsync(Update update)
        {
            var user = await Context.Users.FirstAsync(x => x.TelegramUserId == update.Message.From.Id);
            
            var model = new DbSubmissionsBusinessModel
            {
                ProblemId = user.ProblemId,
                SubmittedQuery = update.Message.Text,
                JudgeTypeId = 1,
                LanguageId = CultureInfo.CurrentCulture.GetCurrentCultureId()
            };

            await AuthenticateAsync(update.Message.From.Id);
            var response = await Client.PostAsJsonAsync(SolvewayApiEndponts.DbCheckSolution, model);

            var responseData = await response.Content.ReadAsJsonAsync<DbValidationResult>();


            DbImageGenerator.GenerateImageFromSolutionQuery(responseData);
            var preparedResponse = PrepareSubmissionStatus(responseData, user.ProblemId);
            

            //update users state in db.
            user.State = ClientStateEnum.None;
            user.ProblemId = 0;
            await Context.SaveChangesAsync();
            
            return new Response
            {
                Message = preparedResponse.ResponseText,
                ChatId = update.Message.Chat.Id,
                ResponseType = ResponseTypeEnum.NewMessage,
                InlineKeyboardMarkup = preparedResponse.InlineKeyboardMarkup,
                ParseMode = ParseMode.Markdown
            };
        }


        public async Task<Response> GetTableResultPictureAsync(Update update)
        {
            var user = await Context.Users.FirstAsync(x => x.TelegramUserId == update.CallbackQuery.From.Id);
            var fs = System.IO.File.OpenRead(@"C:\Users\OlenPC\Desktop\Auca\TelegramBot\test.png");
            return new Response
            {
                ResponseType = ResponseTypeEnum.Photo,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ImageStream = fs
            };
        }

        private PreparedMessageContent PrepareSubmissionStatus(DbValidationResult responseData, int problemId)
        {
            var sb = new StringBuilder();
            if (responseData.Result)
            {
                sb.AppendLine(Resources.QueryCorrect);
            }
            else
            {
                for (var i = 0; i < responseData.Description.Count; i++)
                {
                    if (responseData.Description[i].Contains("SUBMIS0004ER"))
                    {
                        var parsed = responseData.Description[i].Split(',');
                        sb.AppendLine($"The number of rows returned is different on database №-{i + 1}.\n quantity: {parsed[1]}. expected: {parsed[2]}"); //TODO: Ask Ermek
                    }
                    else if (responseData.Description[i].Contains("SUBMIS0003ER"))
                    {
                        var parsed = responseData.Description[i].Split(',');
                        sb.AppendLine($"The number of returned columns is different.\n quantity: {parsed[1]}. expected: {parsed[2]}");
                    }
                    else
                    {
                        sb.AppendLine(responseData.Description[0]);
                    }
                }
            }

            var buttons = new InlineKeyboardButton[1][];
            buttons[0] = new InlineKeyboardButton[2];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData(Resources.TableResultButton, $"{SectionEnums.TableResult}");
            buttons[0][1] = InlineKeyboardButton.WithCallbackData(Resources.SolveAgainButton, $"{SectionEnums.DbProblemSolve} {problemId}");
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }

        private PreparedMessageContent PrepareSingleProblemContent(DbProblemBusinessModel problem)//TODo Localization
        {
            var sb = new StringBuilder();
            var buttons = new InlineKeyboardButton[1][];
            sb.AppendLine($"{problem.ProblemCode} | {problem.ProblemName}");
            sb.AppendLine();
            sb.AppendLine($"{problem.ProblemText}");
            sb.AppendLine();
            buttons[0] = new InlineKeyboardButton[2];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData(Resources.ShowCorrectResultButton, $"{SectionEnums.DbProblemCorrectResult} {problem.Id}");
            buttons[0][1] = InlineKeyboardButton.WithCallbackData(Resources.SolveProblemButton, $"{SectionEnums.DbProblemSolve} {problem.Id}");
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

            sb.AppendLine(Resources.Tasks);
            sb.AppendLine(Resources.ListOfTasksDescription);
            sb.AppendLine();
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
