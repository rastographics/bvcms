using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TouchpointSoftware.Cms.Reporting
{
    public class Report
    {
        public static async Task<ReportResult<TOutput>> FromQueryAsync<TInput, TOutput>(ReportQuery<TInput> query, string connectionString)
            where TInput : ReportParameterModel
            where TOutput : class, new()
        {
            using (var db = new SqlConnection(connectionString))
            {
                await db.OpenAsync();
                var queryResult = await db.QueryAsync<TOutput>(
                    sql: query.CommandText,
                    param: query.Parameters.ToDynamicParameters(),
                    transaction: null,
                    commandTimeout: query.CommandTimeout,
                    commandType: query.CommandType
                );
                var reportResult = new ReportResult<TOutput>(queryResult);
                db.Close();

                return reportResult;
            }
        }
    }
}
