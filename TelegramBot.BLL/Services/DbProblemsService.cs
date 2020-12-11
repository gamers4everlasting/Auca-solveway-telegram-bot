using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BLL.Enums;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Helpers;
using TelegramBot.BLL.Interfaces;
using TelegramBot.BLL.Models;
using TelegramBot.BLL.Models.Contents;
using TelegramBot.BLL.Models.DbProblems;
using TelegramBot.BLL.Models.Generics;
using TelegramBot.BLL.Models.Submissions;
using File = Telegram.Bot.Types.File;

namespace TelegramBot.BLL.Services
{
    public class DbProblemsService: BaseClient, IDbProblemsService
    {
        private readonly string _relativeBasePath;
        private readonly string _virtualBasePath;
        private readonly IHostingEnvironment _env;

        public DbProblemsService(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _relativeBasePath = configuration.GetSection("Files:Path").Value;
            _virtualBasePath = env.ContentRootPath + _relativeBasePath;
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

            // Act
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
                UpdateMessage = false,
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
                UpdateMessage = true,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbProblemsPrevPageAsync(Update update, int currentPage)
        {
            if (currentPage == 1)
            {
                return new Response { UpdateMessage =  null};
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
                UpdateMessage = true,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbProblemsNextPageAsync(Update update, int currentPage, int maxPage)
        {
            if (currentPage == maxPage)
            {
                return new Response { UpdateMessage = null };
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
                UpdateMessage = true,
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
                UpdateMessage = true,
                UpdatingMessageId = update.CallbackQuery.Message.MessageId
            };
        }

        public async Task<Response> GetDbProblemByIdAsync(Update update, int problemId)
        {
            await AuthenticateAsync();
            
            var response = await Client.GetAsync($"/api/DbProblemApi/GetById?problemId={problemId}&languageId=1");

            var problemData =  await response.Content.ReadAsJsonAsync<DbProblemBusinessModel>();

            //create a view for problem data
            var content = PrepareSingleProblemContent(problemData);
            return new Response
            {
                InlineKeyboardMarkup = content.InlineKeyboardMarkup,
                Message = content.ResponseText,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                UpdateMessage = false,
                UpdatingMessageId = 0
            };
        }

        public async Task<Response> PrepareSolveData(Update update, int problemId)
        {
            var data = new JsonDataModel
            {
                TelegramUserId = update.CallbackQuery.From.Id,
                SolveProblemId = problemId
            };

            try
            {
                SaveDataToJson(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new Response
            {
                Message = "Please enter solution: ",
                ChatId = update.CallbackQuery.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                UpdateMessage = false,
                UpdatingMessageId = 0
            };
        }

        private void SaveDataToJson(JsonDataModel data)
        {
            //open file stream
            var json = JsonConvert.SerializeObject(data);
            if (!Directory.Exists(_virtualBasePath))
                Directory.CreateDirectory(_virtualBasePath);

            var fileName = $"{data.TelegramUserId}.json";
            var path = Path.Combine(_virtualBasePath, fileName);


            System.IO.File.WriteAllText(path, json);
        }

        public async Task<Response> ComputeSolution(Update update)
        {
            //get problemId from json
            var fileName = $"{update.Message.From.Id}.json";
            var path = Path.Combine(_virtualBasePath, fileName);
            var content = System.IO.File.ReadAllText(path);
            var d = JsonConvert.DeserializeObject<JsonDataModel>(content);
            if (d.SolveProblemId == 0)
            {
                return new Response{UpdateMessage = null};
            }

            var model = new DbSubmissionsBusinessModel
            {
                ProblemId = d.SolveProblemId,
                SubmittedQuery = update.Message.Text,
                JudgeTypeId = 1,
                LanguageId = 1
            };

            await AuthenticateAsync();
            var response = await Client.PostAsJsonAsync("/api/DbCheckSolution/Create", model);

            var responseData = await response.Content.ReadAsJsonAsync<DbValidationResult>();
            try
            {
                SaveDataToJson(new JsonDataModel { TelegramUserId = update.Message.From.Id, SolveProblemId = 0 });
            }
            catch (Exception e) //TODO: return ExecuteResult
            {
                Console.WriteLine(e);
                throw;
            }

            var preparedResponse = PrepareSubmissionStatus(responseData, d.SolveProblemId);
            

            //set SolveProblemId to 0
            var data = new JsonDataModel
            {
                TelegramUserId = update.Message.From.Id,
                SolveProblemId = 0
            };

            try
            {
                SaveDataToJson(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new Response
            {
                Message = preparedResponse.ResponseText,
                ChatId = update.Message.Chat.Id,
                ParseMode = ParseMode.Default,
                ReplyToMessageId = 0,
                UpdateMessage = false,
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
                        sb.AppendLine($"The number of rows returned is different on Database №-{i + 1}. Quantity: {parsed[1]}. Expected: {parsed[2]}");
                    }
                    else
                    {
                        sb.AppendLine(responseData.Description[0]);
                    }
                }
            }

            var buttons = new InlineKeyboardButton[1][];
            buttons[0] = new InlineKeyboardButton[1];
            buttons[0][0] = InlineKeyboardButton.WithCallbackData("try again", $"{SectionEnums.DbProblemSolve} {problemId}");
            return new PreparedMessageContent
            {
                ResponseText = sb.ToString(),
                InlineKeyboardMarkup = new InlineKeyboardMarkup(buttons)
            };
        }

        private PreparedMessageContent PrepareSingleProblemContent(DbProblemBusinessModel problem)
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

        private PreparedMessageContent PrepareProblemListContent(PagedModel<DbProblemsListBusinessModel> problemsPagedModel, int currentPage)
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
            buttons[index][0] = InlineKeyboardButton.WithCallbackData("1", nameof(PaginationEnums.One));
            buttons[index][1] = InlineKeyboardButton.WithCallbackData("<", nameof(PaginationEnums.PrevPage) + $" {currentPage}");
            buttons[index][2] = InlineKeyboardButton.WithCallbackData($"{currentPage}", nameof(PaginationEnums.CurrentPage));
            buttons[index][3] = InlineKeyboardButton.WithCallbackData(">", nameof(PaginationEnums.NextPage) + $" {currentPage} {lastPage}");
            buttons[index][4] = InlineKeyboardButton.WithCallbackData($"{lastPage}", nameof(PaginationEnums.LastPage) + $" {lastPage}");

            return buttons;
        }
    }
}
