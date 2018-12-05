using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CmsData;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Areas.Dialog.Models
{
    public class NewResourceModel
    {
        public int ResourceId { get; set; }
        [Required(ErrorMessage = "required")]
        public int ResourceTypeId { get; set; }
        [Required(ErrorMessage = "required")]
        public int ResourceCategoryId { get; set; }
        [Required(ErrorMessage = "required")]
        public string Name { get; set; }
        public int? DivisionId { get; set; }
        public IEnumerable<int> OrganizationIds { get; set; } = new List<int>();
        public IEnumerable<int> OrganizationTypeIds { get; set; } = new List<int>();
        public int? CampusId { get; set; }
        public IEnumerable<int> MemberTypeIds { get; set; }
        public IEnumerable<string> StatusFlagIds { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<FileUpload> Files { get; set; }

        public IEnumerable<SelectListItem> Orgs
        {
            get
            {
                var list = DbUtil.Db.Organizations.Select(x => new SelectListItem
                {
                    Text = x.OrganizationName,
                    Value = x.OrganizationId.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> OrgTypes
        {
            get
            {
                var list = DbUtil.Db.OrganizationTypes.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> ResourceTypes
        {
            get
            {
                var list = DbUtil.Db.ResourceTypes
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ResourceTypeId.ToString()
                }).ToList();
                return list;
            }
        }

        public IEnumerable<SelectListItem> ResourceCategories
        {
            get
            {
                var list = DbUtil.Db.ResourceCategories
                .Where(x => x.ResourceTypeId == ResourceTypeId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ResourceCategoryId.ToString()
                }).ToList();
                return list;
            }
        }

        public IEnumerable<SelectListItem> Campuses
        {
            get
            {
                var list = DbUtil.Db.Campus.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> MemberTypes
        {
            get
            {
                var list = DbUtil.Db.MemberTypes.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                return list;
            }
        }

        public IEnumerable<SelectListItem> StatusFlags
        {
            get
            {
                var list = DbUtil.Db.ViewStatusFlagLists.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Flag
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }
        public NewResourceModel() { }
    }

    public class EditResourceModel
    {
        public int ResourceId { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceCategoryId { get; set; }
        public string Name { get; set; }
        public int? DivisionId { get; set; }
        public IEnumerable<int> OrganizationIds { get; set; } = new List<int>();
        public IEnumerable<int> OrganizationTypeIds { get; set; } = new List<int>();
        public int? CampusId { get; set; }
        public IEnumerable<int> MemberTypeIds { get; set; }
        public IEnumerable<string> StatusFlagIds { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        public EditResourceModel()
        {
        }

        public EditResourceModel(Resource r)
        {
            ResourceId = r.ResourceId;
            ResourceTypeId = r.ResourceTypeId;
            ResourceCategoryId = r.ResourceCategoryId;
            Name = r.Name;
            DivisionId = r.DivisionId;
            OrganizationIds = r.ResourceOrganizations.Select(x => x.OrganizationId);
            OrganizationTypeIds = r.ResourceOrganizationTypes.Select(x => x.OrganizationTypeId);
            CampusId = r.CampusId;
            MemberTypeIds = string.IsNullOrWhiteSpace(r.MemberTypeIds) ? new List<int>() : r.MemberTypeIds.Split(',').Select(int.Parse).ToList();
            StatusFlagIds = string.IsNullOrWhiteSpace(r.StatusFlagIds) ? new List<string>() : r.StatusFlagIds.Split(',').ToList();
            Description = r.Description;
            DisplayOrder = r.DisplayOrder ?? 0;
        }

        public IEnumerable<SelectListItem> Orgs
        {
            get
            {
                var list = DbUtil.Db.Organizations.Select(x => new SelectListItem
                {
                    Text = x.OrganizationName,
                    Value = x.OrganizationId.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> OrgTypes
        {
            get
            {
                var list = DbUtil.Db.OrganizationTypes.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> ResourceTypes
        {
            get
            {
                var list = DbUtil.Db.ResourceTypes
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ResourceTypeId.ToString()
                }).ToList();
                return list;
            }
        }

        public IEnumerable<SelectListItem> ResourceCategories
        {
            get
            {
                var list = DbUtil.Db.ResourceCategories
                .Where(x => x.ResourceTypeId == ResourceTypeId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ResourceCategoryId.ToString()
                }).ToList();
                return list;
            }
        }

        public IEnumerable<SelectListItem> Campuses
        {
            get
            {
                var list = DbUtil.Db.Campus.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }

        public IEnumerable<SelectListItem> MemberTypes
        {
            get
            {
                var list = DbUtil.Db.MemberTypes.Select(x => new SelectListItem
                {
                    Text = x.Description,
                    Value = x.Id.ToString()
                }).ToList();

                return list;
            }
        }

        public IEnumerable<SelectListItem> StatusFlags
        {
            get
            {
                var list = DbUtil.Db.ViewStatusFlagLists.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Flag
                }).ToList();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                return list;
            }
        }
    }
}
