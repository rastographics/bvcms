using Dapper;
using System;
using TouchpointSoftware.Cms.Reporting;

namespace CmsWeb.Areas.Finance.Reports
{
    public class TotalByFundReportQuery : ReportQuery<TotalByFundReportLineItem>
    {
        public TotalByFundReportQuery(TotalByFundReportLineItem parameters) : base(parameters)
        {
            CommandText = "";
            CommandType = System.Data.CommandType.StoredProcedure;
            CommandTimeout = 60;
        }
    }

    public class TotalByFundReportLineItem : ReportParameterModel
    {
        public override DynamicParameters ToDynamicParameters()
        {
            throw new NotImplementedException();
        }
    }
}
