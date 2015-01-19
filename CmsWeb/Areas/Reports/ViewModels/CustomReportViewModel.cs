using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    public class CustomReportViewModel
    {
        private readonly List<CustomReportColumn> _columns;
        public string ReportName { get; private set; }

        public IEnumerable<CustomReportColumn> Columns
        {
            get { return _columns; }
        }

        public CustomReportViewModel(IEnumerable<string> standardColumns, string reportName) : this(standardColumns)
        {
            ReportName = reportName;
        }

        public CustomReportViewModel(IEnumerable<string> standardColumns)
        {
            _columns = new List<CustomReportColumn>();
            _columns.AddRange(MapColumns(standardColumns, false));
        }

        public void SetSelectedColumns(IEnumerable<string> columns)
        {
            foreach (var c in _columns.Where(c => columns.Contains(c.Name)))
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