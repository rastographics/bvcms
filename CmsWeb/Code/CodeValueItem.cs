/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class CodeValueItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }

        public string CodeValue => $"{Code}:{Value}";
        public string IdCode => $"{Id},{Code}";
        public string IdValue => $"{Id},{Value}";

        public override string ToString()
        {
            return $"{Id}: {Code}, {Value}";
        }
    }

    public class MemberTypeItem : CodeValueItem
    {
        public int? AttendanceTypeId { get; set; }
    }

    public static class CodeValueItemUtil
    {
        public static string ItemValue(this IEnumerable<CodeValueItem> list, int? id)
        {
            var item = list.FirstOrDefault(i => i.Id == id);
            return item == null ? "(not specified)" : item.Value;
        }

        public static string ItemValue(this IEnumerable<CodeValueItem> list, string id)
        {
            var item = list.FirstOrDefault(i => i.Id == id.ToInt2());
            return item == null ? "(not specified)" : item.Value;
        }

        public static string ItemValue(this IEnumerable<MemberTypeItem> list, int? id)
        {
            var item = list.FirstOrDefault(i => i.Id == id);
            return item == null ? "(not specified)" : item.Value;
        }
    }
}
