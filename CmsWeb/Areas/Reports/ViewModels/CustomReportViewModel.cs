using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    [Serializable]
    public class CustomReportViewModel
    {
        public bool CustomReportSuccessfullySaved { get; set; }

        public int? OrgId { get; set; }

        public string OriginalReportName { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9 ]+$", ErrorMessage = "The report name can only contain alphanumeric characters or spaces. (a-z, 0-9)")]
        public string ReportName { get; set; }

        public List<CustomReportColumn> Columns { get; set; }

        public bool RestrictToThisOrg { get; set; }
        public Guid QueryId { get; set; }
        public string OrgName { get; set; }

        public CustomReportViewModel()
        {
        } // for model binding purposes

        public CustomReportViewModel(int? orgId, Guid queryId, string orgName, IEnumerable<CustomReportColumn> standardColumns, string reportName)
            : this(orgId, queryId, orgName, standardColumns)
        {
            ReportName = reportName;
            OriginalReportName = reportName;
        }

        public CustomReportViewModel(int? orgId, Guid queryId, string orgName, IEnumerable<CustomReportColumn> standardColumns)
        {
            OrgId = orgId;
            QueryId = queryId;
            OrgName = orgName;
            Columns = new List<CustomReportColumn>();
            Columns.AddRange(standardColumns);
        }

        public void SetSelectedColumns(List<CustomReportColumn> columns)
        {
            var nn = columns.Count;
            var n = 0;
            foreach (var column in columns)
            {
                column.Order = n++;
                if (column.IsExtraValue)
                    column.IsDisabled = false;
            }
            var noCol = new CustomReportColumn {Name = "------"};
            var q = from c in Columns
                    join s in columns on c.UniqueName equals s.UniqueName into j
                    from s in j.DefaultIfEmpty(noCol)
                    select new {c, s};
            foreach(var i in q)
                if (i.s.Name == noCol.Name)
                    i.c.Order = nn++;
                else
                {
                    i.c.Order = i.s.Order;
                    i.c.IsSelected = true;
                    i.c.IsDisabled = false;
                }
        }
    }

    [Serializable]
    public class CustomReportColumn
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Field { get; set; }
        public string Flag { get; set; }
        public string OrgId { get; set; }
        public bool IsDisabled { get; set; }
        public string SmallGroup { get; set; }
        public bool IsSelected { get; set; }

        public bool IsStatusFlag => !string.IsNullOrEmpty(Flag);
        public bool IsExtraValue => Name.StartsWith("ExtraValue");
        public bool IsFamilyExtraValue => Name.StartsWith("FamilyExtraValue");
        public bool IsSmallGroup => !string.IsNullOrEmpty(SmallGroup);
        public string UniqueName => IsStatusFlag ? Description : IsExtraValue ? Field : IsSmallGroup ? SmallGroup : Name;

        public int? Order { get; set; }
        public string BindName { get; set; }
    }
}
