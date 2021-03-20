using System;

namespace TelegramBot.Dto.Helper
{
    public class EnumDescriptionAttribute : Attribute
    {
        public string Text { get; set; }

        public EnumDescriptionAttribute(string text)
        {
            Text = text;
        }
    }
}
