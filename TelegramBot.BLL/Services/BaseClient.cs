using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TelegramBot.Dto.Authentication;
using TelegramBot.Dto.Extensions;
using TelegramBot.Dto.Helper;

namespace TelegramBot.BLL.Services
{
    public class BaseClient
    {
        protected readonly HttpClient Client;

        protected BaseClient(IHttpClientFactory clientFactory)
        {
            Client = clientFactory.CreateClient(ApiConstants.ClientName);
        }

        protected async Task AuthenticateAsync() //later (telegramId)
        {
            //get token from telegramId.json check for validation date, if passed, generate new token by userBotId and solveway secretId ->
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync(new JwtAccessTokenRequest { Name = "sergey.petrovsky@timelysoft.net", Password = "123qwe@#WE" }));

        }
        private async Task<string> GetJwtAsync(JwtAccessTokenRequest jwtAccessTokenRequest)
        {
            var url = "api/Token/GetInternalAccessToken";
            var tokenResponse = await Client.PostAsJsonAsync(url, jwtAccessTokenRequest);
            if (!tokenResponse.IsSuccessStatusCode) throw new UnauthorizedAccessException(); //TODO:catch exception on response with description in languages.
            var res = await tokenResponse.Content.ReadAsJsonAsync<JwtTokenInfo>();
            return res.AccessToken;

        }
    }
}
