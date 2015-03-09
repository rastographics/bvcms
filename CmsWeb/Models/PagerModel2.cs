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
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Models
{
    public class PagerModel2
    {
        public PagerModel2(CountDelegate count)
            : this()
        {
            GetCount = count;
        }

        public PagerModel2()
        {
            ShowPageSize = true;
        }

//        public string URL
//        {
//            get
//            {
//                if (!_url.HasValue())
//                    _url = "/{0}/{1}".Fmt(Controller(), Action());
//                return _url;
//            }
//            set { _url = value; }
//        }

        public string Sort { get; set; }
        public string Direction { get; set; }

        public string SortExpression
        {
            get
            {
                if (Direction == "asc")
                    return Sort;
                return Sort + " " + Direction;
            }
        }

        public delegate int CountDelegate();
        public CountDelegate GetCount;
        private int? _count;

        private int count
        {
            get
            {
                if (!_count.HasValue)
                {
                    _count = GetCount();
                    if (StartRow >= _count)
                        _Page = null;
                }
                return _count.Value;
            }
        }

        public void setCountDelegate(CountDelegate count)
        {
            GetCount = new CountDelegate(count);
        }

        public bool ShowPageSize { get; set; }
        public bool AllowSort { get; set; }
        public int? pagesize;
        private readonly int[] pagesizes = { 10, 25, 50, 100, 200 };

        public int PageSize
        {
            get
            {
                if (pagesize.HasValue)
                    return pagesize.Value;
                return DbUtil.Db.UserPreference("PageSize", "10").ToInt();
            }
            set
            {
                if (pagesizes.Contains(value))
                    DbUtil.Db.SetUserPreference("PageSize", value);
                pagesize = value;
            }
        }
        private int? _Page;
        private string _url;

        public int? Page
        {
            get { return _Page ?? 1; }
            set { _Page = value; }
        }
        public int LastPage()
        {
            return (int)Math.Ceiling(count / (double)PageSize);
        }
        public int StartRow
        {
            get { return (Page.Value - 1) * PageSize; }
        }
        public IEnumerable<SelectListItem> PageSizeList()
        {
            return pagesizes.Select(i => new SelectListItem { Text = i.ToString(), Selected = PageSize == i });
        }
        public IEnumerable<int> PageList()
        {
            for (var i = 1; i <= LastPage(); i++)
            {
                if (i > 1 && i < Page - 2)
                {
                    i = Page.Value - 3;
                    yield return 0;
                }
                else if (i < LastPage() && i > Page + 2)
                {
                    i = LastPage() - 1;
                    yield return 0;
                }
                else
                    yield return i;
            }
        }

//        internal void SetWithNoSort(string url, int? page, int? size)
//        {
//            AllowSort = false;
//            URL = url;
//            if (page.HasValue)
//                Page = page.Value;
//            if (size.HasValue)
//                PageSize = size.Value;
//        }
//        internal void SetWithNoPageSize(string url, int? page, string sort, string dir)
//        {
//            ShowPageSize = false;
//            URL = url;
//            if (page.HasValue)
//                Page = page.Value;
//            if (sort.HasValue())
//                Sort = sort;
//            if (dir.HasValue())
//                Direction = dir;
//        }
//        internal void SetWithPageOnly(string url, int? page)
//        {
//            ShowPageSize = false;
//            AllowSort = false;
//            URL = url;
//            if (page.HasValue)
//                Page = page.Value;
//        }
//        public string Controller()
//        {
//            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
//
//            if (routeValues.ContainsKey("controller"))
//                return (string)routeValues["controller"];
//
//            return string.Empty;
//        }
//
//        public string Action()
//        {
//            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
//
//            if (routeValues.ContainsKey("action"))
//                return (string)routeValues["action"];
//
//            return string.Empty;
//        }
//        // All
//        internal void Set(string url, int? page = 1, int? size = null, string sort = null, string dir = null)
//        {
//            URL = url;
//            Set(page, size, sort, dir);
//        }
//        internal void Set(int? page = 1, int? size = null, string sort = null, string dir = null)
//        {
//            AllowSort = true;
//            if (page.HasValue)
//                Page = page.Value;
//            if (size.HasValue)
//                PageSize = size.Value;
//            if (sort.HasValue())
//                Sort = sort ?? "default";
//            if (dir.HasValue())
//                Direction = dir ?? "asc";
//        }
        public HtmlString SortLink(string sortlabel)
        {
            var active = "";
            var asc = " asc";
            var dir = "asc";
            if (sortlabel == Sort)
            {
                active = " active";
                if (Direction == "asc")
                    asc = "";
                dir = Direction == "asc" ? "desc" : "asc";
            }
            return new HtmlString("<a href='#' data-sortby='{0}' data-dir='{1}' class='ajax{2}{3}'>{0}</a>"
                .Fmt(sortlabel, dir, active, asc));
        }
        public HtmlString SortLink2(string label, string html)
        {
            var active = "";
            var asc = " asc";
            var dir = "asc";
            if (label == Sort)
            {
                active = " active";
                if (Direction == "asc")
                    asc = "";
                dir = Direction == "asc" ? "desc" : "asc";
            }
            return new HtmlString("<a href='#' data-sortby='{0}' data-dir='{1}' class='ajax{2}{3}'>{4}</a>"
                .Fmt(label, dir, active, asc, html));
        }
        public HtmlString PageLink(string label, int? page)
        {
            return new HtmlString("<a href='#' data-page='{1}' class='ajax'>{0}</a>"
                .Fmt(label, page ?? 1));
        }
        public HtmlString PageSizeItem(string label, int? size = null, bool? disable = null)
        {
            var disabled = "";
            if (disable == true)
                disabled = " class='disabled'";
            return new HtmlString("<li{2}><a href='#' data-size='{0}' class='ajax'>{1}</a></li>"
                .Fmt(size ?? PageSize, label, disabled));
        }
        public string ShowCount()
        {
            var n = GetCount();
            var cnt = n;
            if (n > PageSize)
                cnt = n - StartRow;
            if (cnt > PageSize)
                cnt = PageSize;
            return "Showing {0} of {1} records".Fmt(cnt, n.ToString("N0"));
        }
    }
}
