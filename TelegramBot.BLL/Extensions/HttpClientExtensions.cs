using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TelegramBot.BLL.Models;

namespace TelegramBot.BLL.Extensions
{
    public static class HttpClientExtensions
    {

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(dataAsString,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        public static async Task<T> ReadAsJsonExecuteResultAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();
            //This is because deserialization cannot be done to ExecuteResult<T>!
            dataAsString = dataAsString.Replace("{\"model\":", "");
            dataAsString = dataAsString.Remove(dataAsString.Length - 1); //removing last }
            var lastIndexOfBrace = dataAsString.LastIndexOf('}');
            dataAsString = dataAsString.Remove(lastIndexOfBrace + 1);
            return JsonSerializer.Deserialize<T>(dataAsString,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        public static async Task<PagedModel<T>> ReadAsJsonPagedModelAsync<T>(this HttpContent content) where T: class
        {
            var dataAsString = await content.ReadAsStringAsync();
            //This is because deserialization cannot be done to PagedModel<T>!
            //dataAsString = dataAsString.Replace("{\"data\":", "");
            //var index = dataAsString.LastIndexOf(',');
            //var totalPages = dataAsString.Substring(index).Replace(",\"total\":", "").Replace("}", "");
            //dataAsString = dataAsString.Remove(index);
            return JsonSerializer.Deserialize<PagedModel<T>>(dataAsString,
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }

        //public static async Task<DbValidationResult> ReadAsJsonDbValidationAsync(this HttpContent content)
        //{
        //    var dataAsString = await content.ReadAsStringAsync();
        //    int firstColonIndex = dataAsString.IndexOf(':') + 1;
        //    string b = dataAsString.Substring(firstColonIndex, dataAsString.IndexOf(',') - firstColonIndex);
        //    bool result = bool.Parse(b);
        //    DbValidationResult model = new DbValidationResult() { Result = result };
        //    return model;
        //}
    }
}
