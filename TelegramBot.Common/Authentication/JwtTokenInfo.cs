using System;

namespace TelegramBot.Dto.Authentication
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
