using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Common.Extensions;
using TelegramBot.Common.Helper;
using TelegramBot.DAL.EF;
using TelegramBot.DAL.Enums;
using TelegramBot.Dto.Authentication;
using TelegramBot.Dto.Helper;

namespace TelegramBot.BLL.Services
{
    public class BaseAuth
    {
        protected readonly HttpClient Client;
        protected readonly ApplicationContext Context;

        protected BaseAuth(IHttpClientFactory clientFactory, ApplicationContext context)
        {
            Context = context;
            Client = clientFactory.CreateClient(ApiConstants.ClientName);
        }

        protected async Task AuthenticateAsync(int telegramUserId)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
            if (user == null || string.IsNullOrEmpty(user.StudyCode) && user.State != ClientStateEnum.SolvewayCodeSet) 
                throw new UnauthorizedAccessException();

            if (user.BearerExpiresIn != null && user.BearerExpiresIn > DateTime.UtcNow && !string.IsNullOrEmpty(user.StudyBearer))
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.StudyBearer);
            }
            else
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync(new JwtAccessTokenRequest { Name = "sergey.petrovsky@timelysoft.net", Password = "123qwe@#WE" }, telegramUserId));
                //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync(new JwtAccessTokenRequest { TelegramUserId = user.TelegramUserId, Code = user.StudyCode }));
            }

        }
        private async Task<string> GetJwtAsync(JwtAccessTokenRequest jwtAccessTokenRequest, int telegramUserId)
        {
            var tokenResponse = await Client.PostAsJsonAsync(SolvewayApiEndponts.AccessTokenURL, jwtAccessTokenRequest);
            if (!tokenResponse.IsSuccessStatusCode) throw new UnauthorizedAccessException();
            var res = await tokenResponse.Content.ReadAsJsonAsync<JwtTokenInfo>();

            var user = await Context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
            user.StudyBearer = res.AccessToken;
            user.BearerExpiresIn = res.Expires;
            if (user.State == ClientStateEnum.SolvewayCodeSet)
                user.State = ClientStateEnum.None;
            await Context.SaveChangesAsync();

            return res.AccessToken;
        }

        //TODO: uncomment when and external api is ready.
        /*protected async Task<string> ValidateClientCode(int telegramUserId, string code)
        {
            var tokenResponse = await Client.PostAsJsonAsync(SolvewayApiEndponts.TelegramAccessTokenURL, telegramUserId, code);
            if (!tokenResponse.IsSuccessStatusCode) return ErrorResources.SolvewayCodeValidationError;
            var res = await tokenResponse.Content.ReadAsJsonAsync<JwtTokenInfo>();

            var user = await Context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
            user.StudyBearer = res.AccessToken;
            user.BearerExpiresIn = res.Expires;
            user.StudyCode = code;
            if (user.State == ClientStateEnum.SolvewayCodeSet)
                user.State = ClientStateEnum.None;
            await Context.SaveChangesAsync();

            return Resources.CodeValidated;
        }*/
        
    }
}
