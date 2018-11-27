using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.Figures.Models
{
    public class GoogleChartsDataTable
    {
        public IList<Col> DataCols { get; } = new List<Col>();
        public IList<Row> DataRows { get; } = new List<Row>();

        public void AddColumn(string label, string type)
        {
            DataCols.Add(new Col() { ColLabel = label, ColType = type });
        }

        public void AddRow(IList<object> values)
        {
            DataRows.Add(new Row() { c = values.Select(x => new Row.RowValue() { v = x }) });
        }

        public class Col
        {
            public string ColLabel { get; set; }
            public string ColType { get; set; }
        }

        public class Row
        {
            public IEnumerable<RowValue> c { get; set; }

            public class RowValue
            {
                public object v;
            }
        }
    }
}
