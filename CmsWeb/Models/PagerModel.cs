/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace CmsWeb.Models
{
    public class PagerModel
    {
        private string _QueryString;
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int LastPage => (int) Math.Ceiling(Count/(double) PageSize);
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool ToggleTarget { get; set; }

        public RouteValueDictionary QueryString
        {
            get { return new RouteValueDictionary(); }
            set
            {
                var sb = new StringBuilder();
                foreach (var v in value)
                    sb.AppendFormat("&{0}={1}", v.Key, v.Value);
                _QueryString = sb.ToString();
            }
        }

        public PagerModel() { }
        public IEnumerable<int> PageSizeList()
        {
            int[] pagesizes = {10, 25, 50, 100, 200};
            return pagesizes.AsEnumerable();
        }

        public IEnumerable<int> PageList()
        {
            for (var i = 1; i <= LastPage; i++)
            {
                if (i > 1 && i < Page - 2)
                {
                    i = Page - 3;
                    yield return 0;
                }
                else if (i < LastPage && i > Page + 2)
                {
                    i = LastPage - 1;
                    yield return 0;
                }
                else
                    yield return i;
            }
        }

        public string PrevLink()
        {
            var prevPage = Page - 1;
            if (Page > 1)
                return $"<a href=\"/{Controller}/{Action}?page={prevPage}{_QueryString}\" title=\"go to page {prevPage}\" class=\"page-numbers prev\">prev </a>";
            return null;
        }

        public string NextLink()
        {
            var nextPage = Page + 1;
            if (Page < LastPage)
                return $"<a href=\"/{Controller}/{Action}?page={nextPage}{_QueryString}\" title=\"go to page {nextPage}\" class=\"page-numbers next\"> next</a>";
            return null;
        }

        public string PageLink(int i)
        {
            if (i == 0)
                return "<span class=\"page-numbers dots\">&hellip;</span>";
            if (i == Page)
                return $"<span class=\"page-numbers current\">{i}</span>";
            return $"<a href=\"/{Controller}/{Action}?page={i}{_QueryString}\" title=\"go to page {i}\" class=\"page-numbers\">{i}</a>";
        }

        public string PageSizeLink(int i)
        {
            var cssClass = i == PageSize ? "current " : "";
            if (i == PageSize)
                return $"<span class=\"current page-numbers\">{i}</span>";
            return $"<a href=\"/{Controller}/{Action}?page={Page}&pagesize={i}{_QueryString}\" title=\"show {i} items per page\" class=\"{cssClass}page-numbers\">{i}</a>";
        }
    }
}
