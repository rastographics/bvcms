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

    public static class ListExtensions
    {
        public static List<T> AddAll<T>(this List<T> self, List<T> list)
        {
            if (list != null)
            {
                foreach(var t in list)
                {
                    self.Add(t);
                }
            }
            return self;
        }
    }
}
