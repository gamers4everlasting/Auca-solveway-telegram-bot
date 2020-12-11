using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot.BLL.Helpers.Enums
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
