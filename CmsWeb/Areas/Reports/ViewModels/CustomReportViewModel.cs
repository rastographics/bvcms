using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    public class CustomReportViewModel
    {
        public int? OrgId { get; set; }

        [Required]
        public string ReportName { get; set; }

        public List<CustomReportColumn> Columns { get; set; }

        public CustomReportViewModel() {} // for model binding purposes

        public CustomReportViewModel(int? orgId, IEnumerable<string> standardColumns, string reportName) : this(orgId, standardColumns)
        {
            ReportName = reportName;
        }

        public CustomReportViewModel(int? orgId, IEnumerable<string> standardColumns)
        {
            OrgId = orgId;
            Columns = new List<CustomReportColumn>();
            Columns.AddRange(MapColumns(standardColumns, false));
        }

        public void SetSelectedColumns(IEnumerable<string> columns)
        {
            foreach (var c in Columns.Where(c => columns.Contains(c.Name)))
            {
                c.IsSelected = true;
            }
        }

        private static IEnumerable<CustomReportColumn> MapColumns(IEnumerable<string> columns, bool isSelected)
        {
            return columns.Select(x => new CustomReportColumn {Name = x, IsSelected = isSelected}).ToList();
        }
    }

    public class CustomReportColumn
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}