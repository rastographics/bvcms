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
        public CMSDataContext CurrentDatabase { get; set; }

        private string filter;
        public string Filter => filter ?? (filter = Util.ContentKeywordFilter ?? "");

        public ContentModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public IQueryable<Content> fetchHTMLFiles() {
            return from c in CurrentDatabase.Contents
                   where c.TypeID == ContentTypeCode.TypeHtml
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }

        public IQueryable<Content> fetchTextFiles()
        {
            return from c in CurrentDatabase.Contents
                   where c.TypeID == ContentTypeCode.TypeText
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }
        public IQueryable<Content> fetchSqlScriptFiles()
        {
            return from c in CurrentDatabase.Contents
                   where c.TypeID == ContentTypeCode.TypeSqlScript
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }
        public IQueryable<Content> fetchPythonScriptFiles()
        {
            return from c in CurrentDatabase.Contents
                   where c.TypeID == ContentTypeCode.TypePythonScript
                   where !Filter.HasValue() || c.ContentKeyWords.Any(vv => vv.Word == Filter)
                   orderby c.Name
                   select c;
        }

        public IQueryable<Content> fetchEmailTemplates()
        {
            return from c in CurrentDatabase.Contents
                   where ContentTypeCode.EmailTemplates.Contains(c.TypeID)
                   orderby c.Name
                   select c;
        }

        public IQueryable<SavedDraft> fetchSavedDrafts()
        {
            return from c in CurrentDatabase.Contents
                   where ContentTypeCode.EmailDrafts.Contains(c.TypeID)
                   from p in CurrentDatabase.Users.Where(p => p.UserId == c.OwnerID).DefaultIfEmpty()
                   from r in CurrentDatabase.Roles.Where(r => r.RoleId == c.RoleID).DefaultIfEmpty()
                   orderby c.Name
                   select new SavedDraft()
                   {
                       created = c.DateCreated,
                       id = c.Id,
                       name = c.Name,
                       owner = p.Username,
                       role = r.RoleName,
                       roleID = c.RoleID,
                       isUnlayer = c.TypeID == ContentTypeCode.TypeUnlayerSavedDraft
                   };
        }

        public static List<Role> fetchRoles(CMSDataContext db)
        {
            var r = from e in db.Roles
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

            var list = (from kw in CurrentDatabase.ContentKeyWords
                        orderby kw.Word
                        select kw.Word).Distinct().ToList();
            var keywordfilter = Util.ContentKeywordFilter;
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

            public bool Owner => ownerID == DbUtil.Db.UserId;
            public bool Shared => !Owner && shared;
            public bool Other => !Shared && roleID != 0;
        }
    }
}
