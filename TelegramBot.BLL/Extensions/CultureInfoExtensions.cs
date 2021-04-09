using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TelegramBot.BLL.Helpers.Enums;

namespace TelegramBot.BLL.Extensions
{
    public static class CultureInfoExtensions
    {
        public static int GetCurrentCultureId(this CultureInfo cultureInfo)
        {
            var culture = cultureInfo.Name;
            if (LanguagesEnum.ru.ToString().Equals(culture))
            {

                return (int)LanguagesEnum.ru;
            }
            return (int)LanguagesEnum.en;
        }
    }
}
