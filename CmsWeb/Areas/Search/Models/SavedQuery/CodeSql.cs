namespace CmsWeb.Areas.Search.Models
{
    public class CodeSql
    {
            public const string Queries = @"
SELECT 
	q.QueryId ,
    q.text ,
    q.owner ,
    q.created ,
    q.lastRun ,
    q.name ,
    q.ispublic ,
    qa.Seconds ,
    qa.OriginalCount ,
    qa.ParsedCount ,
    qa.Message
FROM dbo.Query q
left join QueryAnalysis qa ON qa.Id = q.QueryId
where name <> 'scratchpad'
ORDER BY q.name
";
    }
}