using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Figures.Models
{
    public class OrgViewModel
    {
        public List<OrgModel> Orglist = new List<OrgModel>();
        public Int32 SelectedOrgId { get; set; }

        public IEnumerable<SelectListItem> OrgIEnum
        {
            get
            {
                return new SelectList(Orglist, "ID", "OrgName");
            }
        }

        public OrgViewModel() { }

    }


}
