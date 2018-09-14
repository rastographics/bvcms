using System.Data;

namespace TouchpointSoftware.Cms.Reporting
{

    public class ReportQuery
    {
        public bool HasParameters { get; set; } = false;
        public string CommandText { get; set; } = string.Empty;
        public CommandType CommandType { get; set; } = CommandType.Text;
        public int CommandTimeout { get; set; } = 90;
    }

    public class ReportQuery<TInput> : ReportQuery
        where TInput : ReportParameterModel
    {
        public TInput Parameters { get; set; } = null;

        public ReportQuery(TInput parameters)
            : base()
        {
            Parameters = parameters;
            HasParameters = !(parameters == null);
        }
    }
}
