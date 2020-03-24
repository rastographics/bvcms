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
        public CMSDataContext CurrentDatabase { get; set; }
        public bool WantParents { get; set; }
        public bool CcParents { get; set; }
        public bool NoDups { get; set; }
        public Guid QueryId { get; set; }
        public int? OrgId { get; set; }

        public EmailTemplatesModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }
        public IQueryable<Content> FetchTemplates()
        {
            var currentRoleIds = CurrentDatabase.CurrentRoleIds();
            var isadmin = HttpContextFactory.Current.User.IsInRole("Admin");

            return from i in CurrentDatabase.Contents
                   where ContentTypeCode.EmailTemplates.Contains(i.TypeID)
                   where isadmin || i.RoleID == 0 || currentRoleIds.Contains(i.RoleID)
                   orderby i.Name
                   select i;
        }

        public Content FetchTemplateByName(string name)
        {
            return FetchTemplateByName(name, CurrentDatabase);
        }

        public static Content FetchTemplateByName(string name, CMSDataContext db)
        {
            return (from i in db.Contents
                    where i.Name == name
                    where i.TypeID == ContentTypeCode.TypeEmailTemplate
                    select i).SingleOrDefault();
        }

        private List<ContentModel.SavedDraft> drafts;

        public List<ContentModel.SavedDraft> FetchDrafts()
        {
            var userId = CurrentDatabase.UserId;
            var currentRoleIds = CurrentDatabase.CurrentRoleIds();
            return drafts ?? (drafts =
                   (from c in CurrentDatabase.Contents
                    where ContentTypeCode.EmailDrafts.Contains(c.TypeID)
                    let u = CurrentDatabase.Users.First(vv => vv.UserId == c.OwnerID)
                    let r = CurrentDatabase.Roles.FirstOrDefault(vv => vv.RoleId == c.RoleID)
                    let isshared = (from tt in CurrentDatabase.Tags
                                    where tt.Name == "SharedDrafts"
                                    where tt.PersonOwner.Users.Any(uu => uu.UserId == c.OwnerID)
                                    where tt.PersonTags.Any(vv => vv.PeopleId == CurrentDatabase.UserPeopleId)
                                    select tt.Id).Any()
                    where c.RoleID == 0 || c.OwnerID == userId || currentRoleIds.Contains(c.RoleID)
                    orderby (c.OwnerID == userId ? 1 : 0) descending, c.Name
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
