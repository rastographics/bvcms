using CmsData;
using CmsData.Codes;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Models
{
    public class EmailTemplatesModel
    {
        public bool WantParents { get; set; }
        public bool CcParents { get; set; }
        public bool NoDups { get; set; }
        public Guid QueryId { get; set; }
        public int? OrgId { get; set; }

        public EmailTemplatesModel() { }
        public IQueryable<Content> FetchTemplates()
        {
            var currentRoleIds = DbUtil.Db.CurrentRoleIds();
            var isadmin = HttpContextFactory.Current.User.IsInRole("Admin");

            return from i in DbUtil.Db.Contents
                   where i.TypeID == ContentTypeCode.TypeEmailTemplate
                   where isadmin || i.RoleID == 0 || currentRoleIds.Contains(i.RoleID)
                   orderby i.Name
                   select i;
        }

        public Content FetchTemplateByName(string name)
        {
            return (from i in DbUtil.Db.Contents
                    where i.Name == name
                    select i).SingleOrDefault();
        }

        private List<ContentModel.SavedDraft> drafts;

        public List<ContentModel.SavedDraft> FetchDrafts()
        {
            var currentRoleIds = DbUtil.Db.CurrentRoleIds();
            return drafts ?? (drafts =
                   (from c in DbUtil.Db.Contents
                    where c.TypeID == ContentTypeCode.TypeSavedDraft || c.TypeID == ContentTypeCode.TypeUnlayerSavedDraft
                    let u = DbUtil.Db.Users.First(vv => vv.UserId == c.OwnerID)
                    let r = DbUtil.Db.Roles.FirstOrDefault(vv => vv.RoleId == c.RoleID)
                    let isshared = (from tt in DbUtil.Db.Tags
                                    where tt.Name == "SharedDrafts"
                                    where tt.PersonOwner.Users.Any(uu => uu.UserId == c.OwnerID)
                                    where tt.PersonTags.Any(vv => vv.PeopleId == Util.UserPeopleId)
                                    select tt.Id).Any()
                    where c.RoleID == 0 || c.OwnerID == Util.UserId || currentRoleIds.Contains(c.RoleID)
                    orderby (c.OwnerID == Util.UserId ? 1 : 0) descending, c.Name
                    select new ContentModel.SavedDraft()
                    {
                        created = c.DateCreated,
                        id = c.Id,
                        name = c.Name,
                        shared = isshared,
                        owner = u.Person.Name,
                        ownerID = c.OwnerID,
                        role = r.RoleName,
                        roleID = c.RoleID,
                        isUnlayer = c.TypeID == ContentTypeCode.TypeUnlayerSavedDraft
                    }).ToList());
        }
    }
}
