using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.BLL.Extensions;
using TelegramBot.BLL.Models.Authentication;

namespace TelegramBot.BLL.Services
{
    public class BaseClient
    {
        public HttpClient Client;
        public string Token { get; private set; }
        public BaseClient()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri("https://api.solveway.club/")
            };
        }
        public async Task AuthenticateAsync() //later (telegramId)
        {
            //get token from telegramId.json check for validation date, if passed, generate new token ->
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync(new JwtAccessTokenRequest { Name = "sergey.petrovsky@timelysoft.net", Password = "123qwe@#WE" }));

        }
        private async Task<string> GetJwtAsync(JwtAccessTokenRequest jwtAccessTokenRequest)
        {
            var url = "api/Token/GetInternalAccessToken";
            var tokenResponse = await Client.PostAsJsonAsync(url, jwtAccessTokenRequest);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var res = await tokenResponse.Content.ReadAsJsonAsync<JwtTokenInfo>();
                Token = res.AccessToken;
                return res.AccessToken;
            }

            return null;
        }
    }
}
