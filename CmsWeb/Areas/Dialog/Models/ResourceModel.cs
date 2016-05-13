using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CmsData;

namespace CmsWeb.Areas.Dialog.Models
{
    public class NewResourceModel
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public int OrgId { get; set; }
        public FileUpload File { get; set; }

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

        public IEnumerable<SelectListItem> Campuses
        {
            get
            {
                var list = new List<SelectListItem>();

                list.Insert(0, new SelectListItem { Value = "0", Text = "(none)", Selected = true });
                list.Insert(0, new SelectListItem { Value = "1", Text = "West Side", Selected = true });
                return list;
            }
        }

        public IEnumerable<MemberType> MemberTypes
        {
            get { return DbUtil.Db.MemberTypes; }
        } 
    }
}