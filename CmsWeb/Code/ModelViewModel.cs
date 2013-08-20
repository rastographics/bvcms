using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class CodeValue : Attribute { }
    public class TrackChanges : Attribute { }
    public class ZeroToNull : Attribute { }

    public static class ModelViewModel
    {
        public static void CopyPropertiesFrom(this object target, object source, bool CodeValuesOnly = false, bool TrackChanges = false)
        {
            var cv = new CodeValueModel();
            var sourceProps = source.GetType().GetProperties();
            var targetProps = target.GetType().GetProperties();
            foreach (var s in sourceProps)
            {
                // does the source have CodeValue attribute? (don't copy that direction)
                if (Attribute.IsDefined(s, typeof (CodeValue)))
                    continue;

                // find a target property of the same name as source
                var t = targetProps.FirstOrDefault(tt => tt.Name == s.Name);
                if (t == null)
                    continue;

                if (Attribute.IsDefined(t, typeof (TrackChanges)))
                    continue;

                var isTargetCodeValue = Attribute.IsDefined(t, typeof(CodeValue));
                if (CodeValuesOnly && !isTargetCodeValue)
                    continue;

                // get the source value we are going to copy
                var so = s.GetValue(source, null);

                // if they are the same type, then straight copy
                if (s.PropertyType == t.PropertyType)
                    t.SetValue(target, so, null);

                else if (isTargetCodeValue)
                {
                    /* This code uses a convention to map a lookup table description
                     * to a string. The method to get the lookup table must be a method
                     * in the CodeValueModel class with the same name as the field being
                     * populated but with "List" on the end. Also it is assumed that there 
                     * is a correpsponding int Id property in the source with the same name 
                     * followed by "Id"
                     */
                    var getlist = cv.GetType().GetMethod(t.Name + "List");
                    var list = (IEnumerable<CodeValueItem>)getlist.Invoke(cv, null);
                    var tid = sourceProps.FirstOrDefault(ss => ss.Name == s.Name + "Id");
                    if (tid != null)
                    {
                        var sv = tid.GetValue(source, null) ?? 0;
                        var v = list.ItemValue((int)sv);
                        t.SetPropertyFromText(target, v);
                    }
                }
                else if (Attribute.IsDefined(s, typeof (ZeroToNull)))
                {
                    if((int) so == 0)
                        t.SetValue(target, null, null);
                }
                else if (so is string)
                    t.SetPropertyFromText(target, (string) so);

                else if (so is DateTime && (DateTime) so == ((DateTime) so).Date)
                    t.SetPropertyFromText(target, ((DateTime) so).ToShortDateString());


                else // Handle any other type mismatches like int = Nullable<int> or vice-versa
                    t.SetPropertyFromText(target, (so ?? "").ToString());
            }
        }
        public static SelectList ToSelect(this IEnumerable<CodeValueItem> items)
        {
            if (items == null)
                throw new Exception("items are null in SelectList");
            return new SelectList(items, "Id", "Value");
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q)
        {
            return q.AddNotSpecified(0);
        }
        public static IEnumerable<CodeValueItem> AddNotSpecified(this IEnumerable<CodeValueItem> q, int value)
        {
            var list = q.ToList();
            list.Insert(0, new CodeValueItem { Id = value, Code = value.ToString(), Value = "(not specified)" });
            return list;
        }
    }
}