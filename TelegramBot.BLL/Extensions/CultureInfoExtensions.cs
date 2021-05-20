using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TelegramBot.BLL.Helpers.Enums;
using TelegramBot.DAL.Enums;

namespace TelegramBot.BLL.Extensions
{
    public static class CultureInfoExtensions
    {
        public static int GetCurrentCultureId(this CultureInfo cultureInfo)
        {
            var culture = cultureInfo.Name; //TODO: Get the current language from database!.
            if (LanguagesEnum.Ru.GetDescription().Equals(culture))
                return (int)LanguagesEnum.Ru;
            if(LanguagesEnum.Ky.GetDescription().Equals(culture))
                return (int)LanguagesEnum.Ky;
            return (int)LanguagesEnum.En;
        }
    }
}
