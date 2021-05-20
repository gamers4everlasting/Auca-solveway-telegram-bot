using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TelegramBot.BLL.Extensions
{
    /// <summary>
    /// Enum extensions
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get description of enum
        /// </summary>
        public static string GetDescription<TEnum>(this TEnum enumerationValue) where TEnum : Enum
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum) throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type");
            
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        /// <summary>
        /// Gets description of enum or null (for nullable enums)
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescriptionOrNull(this Enum enumValue)
        {
            return enumValue == null ? null : GetDescription(enumValue);
        }
        
        /// <summary>
        /// Get all values of the given enum as Enumerable
        /// </summary>
        public static IEnumerable<TEnum> GetAllAsEnumerable<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
        }
    }
}