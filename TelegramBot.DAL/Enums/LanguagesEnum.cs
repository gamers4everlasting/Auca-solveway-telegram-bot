using System.ComponentModel;

namespace TelegramBot.DAL.Enums
{
    public enum LanguagesEnum
    {
        None,
        [Description("ru-RU")]
        Ru,
        [Description("en-US")]
        En,
        [Description("ky-KG")]
        Ky
    }
}