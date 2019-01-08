using CmsData;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models
{
    public class ResourceModel
    {
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public List<ResourceAttachment> Attachments { get; set; }

        public ResourceModel() { }

        public ResourceModel(int resourceId)
        {
            ResourceId = resourceId;
            Resource = DbUtil.Db.Resources.First(x => x.ResourceId == resourceId);
            Attachments = DbUtil.Db.ResourceAttachments.Where(x => x.ResourceId == resourceId).ToList();
        }

        public ResourceModel(Resource resource)
        {
            Resource = resource;
            ResourceId = resource.ResourceId;
            Attachments = DbUtil.Db.ResourceAttachments.Where(x => x.ResourceId == ResourceId).ToList();
        }

        [DisplayName("Organizations")]
        public string OrganizationNames
            => string.Join(", ", Resource.ResourceOrganizations.Select(ro => ro.Organization.OrganizationName));

        [DisplayName("Organization Types")]
        public string OrganizationTypeNames
            => string.Join(", ", Resource.ResourceOrganizationTypes.Select(rot => rot.OrganizationType.Description));

        [DisplayName("Congregation")]
        public string CampusName => Resource.Campu?.Description;

        [DisplayName("Member Types")]
        public string MemberTypes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Resource.MemberTypeIds))
                {
                    return string.Empty;
                }

                var memberTypeIds = Resource.MemberTypeIds.Split(',').Select(int.Parse);
                return string.Join(", ", DbUtil.Db.MemberTypes.Where(x => memberTypeIds.Contains(x.Id)).Select(x => x.Description));
            }
        }

        [DisplayName("Status Flags")]
        public string StatusFlags
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Resource.StatusFlagIds))
                {
                    return string.Empty;
                }

                var statusFlagIds = Resource.StatusFlagIds.Split(',');
                return string.Join(", ", DbUtil.Db.ViewStatusFlagLists.Where(x => statusFlagIds.Contains(x.Flag)).Select(x => x.Name));
            }
        }

        [DisplayName("Resource Type")]
        public string ResourceTypeName => Resource.ResourceType.Name;

        [DisplayName("Resource Category")]
        public string ResourceCategoryName => Resource.ResourceCategory.Name;
    }
}
