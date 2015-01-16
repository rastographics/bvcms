using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    public class CustomReportViewModel
    {
        public IEnumerable<CustomReportColumn> Columns { get; private set; }

        public CustomReportViewModel(IEnumerable<string> columns)
        {
            Columns = columns.Select(x => new CustomReportColumn {Name = x}).ToList();
        }
    }

    public class CustomReportColumn
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}