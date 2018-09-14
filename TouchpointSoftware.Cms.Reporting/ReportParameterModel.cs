using Dapper;

namespace TouchpointSoftware.Cms.Reporting
{
    public abstract class ReportParameterModel
    {
        public abstract DynamicParameters ToDynamicParameters();
    }
}
