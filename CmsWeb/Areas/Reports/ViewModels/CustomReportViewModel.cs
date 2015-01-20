using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    public class CustomReportViewModel
    {
        public int? OrgId { get; set; }

        public string OriginalReportName { get; set; }

        [Required]
        public string ReportName { get; set; }

        public List<CustomReportColumn> Columns { get; set; }

        public bool RestrictToThisOrg { get; set; }

        public CustomReportViewModel() {} // for model binding purposes

        public CustomReportViewModel(int? orgId, IEnumerable<CustomReportColumn> standardColumns, string reportName) : this(orgId, standardColumns)
        {
            ReportName = reportName;
            OriginalReportName = reportName;
        }

        public CustomReportViewModel(int? orgId, IEnumerable<CustomReportColumn> standardColumns)
        {
            OrgId = orgId;
            Columns = new List<CustomReportColumn>();
            Columns.AddRange(standardColumns);
        }

        public void SetSelectedColumns(IEnumerable<CustomReportColumn> columns)
        {
            foreach (var column in Columns)
            {
                if (column.IsStatusFlag)
                {
                    if (columns.Select(c => c.Description).Contains(column.Description))
                        column.IsSelected = true;
                }
                else
                {
                    if (columns.Select(c => c.Name).Contains(column.Name))
                        column.IsSelected = true;
                }
            }
        }
    }

    public class CustomReportColumn
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
        public string OrgId { get; set; }

        public bool IsStatusFlag { get { return !string.IsNullOrEmpty(Flag); } }
    }
}