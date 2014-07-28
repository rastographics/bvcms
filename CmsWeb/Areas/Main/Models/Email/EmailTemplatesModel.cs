using System;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models
{
	public class EmailTemplatesModel
	{
	    public bool wantparents { get; set; }
	    public bool ccparents { get; set; }
	    public Guid queryid { get; set; }
		public IQueryable<Content> fetchTemplates()
		{
			var currentRoleIds = DbUtil.Db.CurrentRoleIds();
			var isadmin = HttpContext.Current.User.IsInRole("Admin");

			return from i in DbUtil.Db.Contents
					 where i.TypeID == ContentTypeCode.TypeEmailTemplate
					 where isadmin || i.RoleID == 0 || currentRoleIds.Contains(i.RoleID)
					 orderby i.Name
					 select i;
		}

        public Content fetchTemplateByName( string name )
        {
            var currentRoleIds = DbUtil.Db.CurrentRoleIds();
            var isadmin = HttpContext.Current.User.IsInRole("Admin");

            return (from i in DbUtil.Db.Contents
                    where i.Name == name
                    select i).SingleOrDefault();
        }

        public IQueryable<ContentModel.SavedDraft> fetchDrafts()
		{
			var currentRoleIds = DbUtil.Db.CurrentRoleIds();
			var isadmin = HttpContext.Current.User.IsInRole("Admin");

            return from c in DbUtil.Db.Contents
                   where c.TypeID == ContentTypeCode.TypeSavedDraft
                   from u in DbUtil.Db.Users.Where(u => u.UserId == c.OwnerID).DefaultIfEmpty()
                   from p in DbUtil.Db.People.Where(p => p.PeopleId == u.PeopleId).DefaultIfEmpty()
                   from r in DbUtil.Db.Roles.Where(r => r.RoleId == c.RoleID).DefaultIfEmpty()
                   where c.RoleID == 0 || c.OwnerID == Util.UserId || currentRoleIds.Contains(c.RoleID)
                   orderby (c.OwnerID == Util.UserId ? 1 : 0) descending, c.Name
                   select new ContentModel.SavedDraft()
                   {
                       created = c.DateCreated,
                       id = c.Id,
                       name = c.Name,
                       owner = p.Name,
                       ownerID = c.OwnerID,
                       role = r.RoleName,
                       roleID = c.RoleID
                   };
		}
	}
}