using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Figures.Models
{
    public class DivViewModel
    {
        public List<DivModel> Divlist = new List<DivModel>();
        public Int32 SelectedDivId { get; set; }

        public IEnumerable<SelectListItem> DivIEnum
        {
            get
            {
                return new SelectList(Divlist, "ID", "DivName");
            }
        }

        public DivViewModel() { }

    }
}
