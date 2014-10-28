using System;

namespace UtilityExtensions.Extensions
{
    public static class Extensions
    {
        public static T ParseEnum<T>(this string enumValue)
        {
            return (T) Enum.Parse(typeof(T), enumValue);
        }
    }
}
