using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ContentModel
    {
        private string filter;
        public string Filter => filter ?? (filter = HttpContextFactory.Current.Session["ContentKeywordFilter"] as string ?? "");
        public ContentModel() { }
        public IQueryable<Content> fetchHTMLFiles()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeHtml
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }

        public IQueryable<Content> fetchTextFiles()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeText
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }
        public IQueryable<Content> fetchSqlScriptFiles()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeSqlScript
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }
        public IQueryable<Content> fetchPythonScriptFiles()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypePythonScript
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }

        public IQueryable<Content> fetchEmailTemplates()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeEmailTemplate
                   orderby c.Name
                   select c;
        }

        public IQueryable<SavedDraft> fetchSavedDrafts()
        {
            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeSavedDraft
                   from p in DbUtil.Db.Users.Where(p => p.UserId == c.OwnerID).DefaultIfEmpty()
                   from r in DbUtil.Db.Roles.Where(r => r.RoleId == c.RoleID).DefaultIfEmpty()
                   orderby c.Name
                   select new SavedDraft()
                   {
                       created = c.DateCreated,
                       id = c.Id,
                       name = c.Name,
                       owner = p.Username,
                       role = r.RoleName,
                       roleID = c.RoleID
                   };
        }

        public static List<Role> fetchRoles()
        {
            var r = from e in DbUtil.Db.Roles
                    select e;

            var l = r.ToList();
            l.Insert(0, new Role() { RoleId = 0, RoleName = "Everyone" });
            return l;
        }

        private List<SelectListItem> keywords;
        public List<SelectListItem> KeywordFilters()
        {
            if (keywords != null)
            {
                return keywords;
            }

            var list = (from kw in DbUtil.Db.ContentKeyWords
                        orderby kw.Word
                        select kw.Word).Distinct().ToList();
            var keywordfilter = HttpContextFactory.Current.Session["ContentKeywordFilter"] as string;
            keywords = list.Select(vv => new SelectListItem() { Text = vv, Value = vv, Selected = vv == keywordfilter }).ToList();
            keywords.Insert(0, new SelectListItem() { Text = "(not specified)", Value = "" });
            return keywords;
        }


        public class SavedDraft
        {
            public int id { get; set; }
            public string name { get; set; }
            public string owner { get; set; }
            public int ownerID { get; set; }
            public string role { get; set; }
            public int roleID { get; set; }
            public DateTime? created { get; set; }
            public bool shared { get; set; }
            public bool isUnlayer { get; set; }

            public bool Owner => ownerID == Util.UserId;
            public bool Shared => !Owner && shared;
            public bool Other => !Shared && roleID != 0;
        }
    }
}
