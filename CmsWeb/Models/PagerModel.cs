/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UtilityExtensions;
using System.Text;

namespace CmsWeb.Models
{
    public class PagerModel
    {
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int LastPage
        {
            get { return (int)Math.Ceiling(Count / (double)PageSize); }
        }
        public string Action { get; set; }
        public string Controller { get; set; }
		public bool ToggleTarget { get; set; }

        private string _QueryString;
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

        public IEnumerable<int> PageSizeList()
        {
            int[] pagesizes = { 10, 25, 50, 100, 200 };
            return pagesizes.AsEnumerable();
        }
        public IEnumerable<int> PageList()
        {
            for (var i = 1; i <= LastPage;i++ )
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
            if (Page > 1)
                return "<a href=\"/{1}/{2}?page={0}{3}\" title=\"go to page {0}\" class=\"page-numbers prev\">prev </a>"
                    .Fmt(Page -1, Controller, Action, _QueryString);
            return null;
        }
        public string NextLink()
        {
            if (Page < LastPage)
                return "<a href=\"/{1}/{2}?page={0}{3}\" title=\"go to page {0}\" class=\"page-numbers next\"> next</a>"
                    .Fmt(Page +1, Controller, Action, _QueryString);
            return null;
        }
        public string PageLink(int i)
        {
            if (i == 0)
                return "<span class=\"page-numbers dots\">&hellip;</span>";
            if (i == Page)
                return "<span class=\"page-numbers current\">{0}</span>".Fmt(i);
            return "<a href=\"/{1}/{2}?page={0}{3}\" title=\"go to page {0}\" class=\"page-numbers\">{0}</a>"
                .Fmt(i, Controller, Action, _QueryString);
        }
        public string PageSizeLink(int i)
        {
            if (i == PageSize)
                return "<span class=\"current page-numbers\">{0}</span>".Fmt(i);
            return "<a href=\"/{1}/{2}?page={3}&pagesize={0}{4}\" title=\"show {0} items per page\" class=\"{5}page-numbers\">{0}</a>"
                .Fmt(i, Controller, Action, Page, _QueryString, i==PageSize? "current " : "");
       }
    }
}
