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
        public int? OrganizationId { get; set; }
        public int? CampusId { get; set; }
        public string MemberTypeIds { get; set; }
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

        public IEnumerable<MemberType> MemberTypes
        {
            get { return DbUtil.Db.MemberTypes; }
        } 
    }

    public class EditResourceModel
    {
        public int ResourceId { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceCategoryId { get; set; }
        public string Name { get; set; }
        public int? DivisionId { get; set; }
        public int? OrganizationId { get; set; }
        public int? CampusId { get; set; }
        public string MemberTypeIds { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        public EditResourceModel()
        {
            
        }

        public EditResourceModel(Resource r)
        {
            ResourceId = r.ResourceId;
            Name = r.Name;
            DivisionId = r.DivisionId;
            OrganizationId = r.OrganizationId;
            CampusId = r.CampusId;
            MemberTypeIds = r.MemberTypeIds;
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

        public IEnumerable<MemberType> MemberTypes
        {
            get { return DbUtil.Db.MemberTypes; }
        }
    }
}