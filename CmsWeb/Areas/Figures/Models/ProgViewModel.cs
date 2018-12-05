using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Figures.Models
{
    public class ProgViewModel
    {
        public List<ProgModel> Proglist = new List<ProgModel>();
        public Int32 SelectedProgId { get; set; }

        public IEnumerable<SelectListItem> ProgIEnum
        {
            get
            {
                return new SelectList(Proglist, "ID", "ProgName");
            }
        }

        public ProgViewModel() { }

    }

}
