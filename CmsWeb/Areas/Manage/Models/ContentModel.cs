using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Controllers;
using UtilityExtensions;

namespace CmsWeb.Models
{
	public class ContentModel
	{
		public IQueryable<Content> fetchHTMLFiles()
		{
			return from c in DbUtil.Db.Contents
					 where c.TypeID == ContentTypeCode.TypeHtml
					 orderby c.Name
					 select c;
		}

		public IQueryable<Content> fetchTextFiles()
		{
			return from c in DbUtil.Db.Contents
					 where c.TypeID == ContentTypeCode.TypeText
					 orderby c.Name
					 select c;
		}
		public IQueryable<Content> fetchSqlScriptFiles()
		{
			return from c in DbUtil.Db.Contents
					 where c.TypeID == ContentTypeCode.TypeSqlScript
					 orderby c.Name
					 select c;
		}
		public IQueryable<Content> fetchPythonScriptFiles()
		{
			return from c in DbUtil.Db.Contents
					 where c.TypeID == ContentTypeCode.TypePythonScript
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
                     from p in DbUtil.Db.Users.Where( p => p.UserId == c.OwnerID ).DefaultIfEmpty()
                     from r in DbUtil.Db.Roles.Where( r => r.RoleId == c.RoleID ).DefaultIfEmpty()
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
			l.Insert( 0, new Role() { RoleId = 0, RoleName = "Everyone" } );
			return l;
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

            public bool Owner => ownerID == Util.UserId;
            public bool Shared => !Owner && shared;
            public bool Other => !Shared && roleID != 0;
        }
	}
}