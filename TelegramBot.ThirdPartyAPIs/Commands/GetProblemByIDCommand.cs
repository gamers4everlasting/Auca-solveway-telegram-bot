using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TelegramBot.Dto.DbProblemModels;
using TelegramBot.Dto.Extensions;
using TelegramBot.ThirdPartyAPIs.Interfaces;
using TelegramBot.ThirdPartyAPIs.Models;

namespace TelegramBot.ThirdPartyAPIs.Commands
{
    public class GetProblemByIDCommand : ICommand
    {
        private readonly HttpClient _client;
        private readonly string _url;

        public GetProblemByIDCommand(int problemId, int languageId, HttpClient client) //put bearer on initialization;
        {
            _client = client;
            _url = $"/api/DbProblemApi/GetById?problemId={problemId}&languageId={languageId}";
        }


        public async Task<CommandResult> Execute()
        {
            var urlResponse = await _client.GetAsync(_url);
            //check for error
            var data = await urlResponse.Content.ReadAsJsonAsync<DbProblemBusinessModel>();
            return new CommandResult
            {
                Data = Enumerable.Repeat(data, 1)
            };
        }
    }
}