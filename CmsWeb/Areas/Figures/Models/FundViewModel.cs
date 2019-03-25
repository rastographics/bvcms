using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Figures.Models
{
    public class FundViewModel
    {
        public List<FundModel> Fundlist = new List<FundModel>();
        public Int32 SelectedFundId { get; set; }

        public IEnumerable<SelectListItem> FundIEnum
        {
            get
            {
                return new SelectList(Fundlist, "ID", "FundName");
            }
        }
        public FundViewModel() { }
    }
}
