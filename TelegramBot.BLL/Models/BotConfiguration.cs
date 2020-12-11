using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramBot.BLL.Models
{
    public class BotConfiguration
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int Port { get; set; }
    }
}
