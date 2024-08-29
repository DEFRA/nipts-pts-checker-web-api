using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.Helper
{
    [ExcludeFromCodeCoverage]
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            return enumValue.ToString();
        }
    }
}
