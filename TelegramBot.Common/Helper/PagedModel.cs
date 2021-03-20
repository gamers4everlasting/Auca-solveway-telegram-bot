using System.Collections.Generic;

namespace TelegramBot.Dto.Helper
{
    public class PagedModel<T> where T : class
    {
        public IList<T> Data { get; set; }
        public int Total { get; set; }

        public PagedModel()
        {
            Data = new List<T>();
        }
    }
}
