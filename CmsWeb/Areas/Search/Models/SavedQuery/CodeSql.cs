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
    q.ispublic
FROM dbo.Query q
where name <> 'scratchpad'
ORDER BY q.name
";
    }
}
