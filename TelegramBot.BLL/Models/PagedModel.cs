using System;
using System.Collections.Generic;
using System.Text;
using X.PagedList;

namespace TelegramBot.BLL.Models
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
