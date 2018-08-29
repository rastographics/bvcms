using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace UtilityExtensions.Extensions
{
    public static class Extensions
    {
        public static T ParseEnum<T>(this string enumValue)
        {
            return (T) Enum.Parse(typeof(T), enumValue);
        }
    }

    public static class XmlExtensions
    {
        public static string AttributeOrNull(this XElement element, string attributeName)
        {
            var attr = element.Attribute(attributeName);
            return attr == null ? null : attr.Value;
        }
    }
}
