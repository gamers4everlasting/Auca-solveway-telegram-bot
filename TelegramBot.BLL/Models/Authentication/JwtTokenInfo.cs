using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.BLL.Models.Authentication
{
    public class JwtTokenInfo
    {
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string Issued { get; set; }
        public DateTime Expires { get; set; }
    }
}
