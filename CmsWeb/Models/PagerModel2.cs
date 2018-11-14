/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PagerModel2
    {
        public delegate int CountDelegate();

        private readonly int[] pagesizes = { 10, 25, 50, 100, 200 };
        private int? _count;
        private int? _Page;
        public int DisplayCount = 0;
        public CountDelegate GetCount;
        public int? pagesize;

        public PagerModel2(CountDelegate count)
            : this()
        {
            GetCount = count;
        }

        public PagerModel2()
        {
            ShowPageSize = true;
        }

        public string Sort { get; set; }
        public string Direction { get; set; }
        public bool AjaxPager { get; set; }

        public string SortExpression
        {
            get
            {
                if (Direction == "asc")
                {
                    return Sort;
                }

                return Sort + " " + Direction;
            }
        }

        private int count
        {
            get
            {
                if (!_count.HasValue)
                {
                    _count = GetCount();
                    if (StartRow >= _count)
                    {
                        _Page = null;
                    }
                }
                return _count.Value;
            }
        }

        public bool ShowPageSize { get; set; }
        public bool AllowSort { get; set; }

        public int PageSize
        {
            get
            {
                if (pagesize.HasValue)
                {
                    return pagesize.Value;
                }

                pagesize = DbUtil.Db.UserPreference("PageSize", "10").ToInt();
                return pagesize.Value;
            }
            set
            {
                if (pagesizes.Contains(value))
                {
                    DbUtil.Db.SetUserPreference("PageSize", value);
                }

                pagesize = value;
            }
        }

        public int? Page
        {
            get { return _Page ?? 1; }
            set { _Page = value; }
        }

        public int StartRow => (Page.Value - 1) * PageSize;

        public void setCountDelegate(CountDelegate count)
        {
            GetCount = count;
        }

        public int LastPage()
        {
            return (int)Math.Ceiling(count / (double)PageSize);
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
                {
                    yield return i;
                }
            }
        }

        public HtmlString SortLink(string sortLabel, string fieldName = null)
        {
            fieldName = fieldName ?? sortLabel;

            var active = "";
            var asc = " asc";
            var dir = "asc";
            if (sortLabel == Sort)
            {
                active = " active";
                if (Direction == "asc")
                {
                    asc = "";
                }

                dir = Direction == "asc" ? "desc" : "asc";
            }
            return new HtmlString($"<a href='#' data-sortby='{fieldName}' data-dir='{dir}' class='ajax{active}{asc}'>{sortLabel}</a>");
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
                {
                    asc = "";
                }

                dir = Direction == "asc" ? "desc" : "asc";
            }
            return new HtmlString($"<a href='#' data-sortby='{label}' data-dir='{dir}' class='ajax{active}{asc}'>{html}</a>");
        }

        public HtmlString PageLink(string label, int? page)
        {
            return new HtmlString($"<a href='#' data-page='{page ?? 1}' class='ajax'>{label}</a>");
        }

        public HtmlString PageSizeItem(string label, int? size = null, bool? disable = null)
        {
            var disabled = "";
            if (disable == true)
            {
                disabled = " class='disabled'";
            }

            return new HtmlString($"<li{disabled}><a href='#' data-size='{size ?? PageSize}' class='ajax'>{label}</a></li>");
        }

        public string ShowCount()
        {
            var n = GetCount();
            var cnt = n;
            if (n > PageSize)
            {
                cnt = n - StartRow;
            }

            if (cnt > PageSize)
            {
                cnt = PageSize;
            }

            return $"Showing {cnt} of {n.ToString("N0")} records";
        }
    }
}
