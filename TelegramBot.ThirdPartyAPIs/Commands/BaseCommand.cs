using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TelegramBot.ThirdPartyAPIs.Commands
{
    public abstract class BaseCommand
    {
        private readonly HttpClient _client;

        protected BaseCommand(HttpClient client)
        {
            _client = client;
        }

    }
}