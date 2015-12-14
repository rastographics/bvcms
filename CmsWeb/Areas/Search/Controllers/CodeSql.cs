namespace CmsWeb.Areas.Search.Controllers
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
ORDER BY q.lastRun desc
";
#if DEBUG
        public const string Populate = @"
INSERT dbo.QueryAnalysis ( Id )
SELECT q.QueryId 
FROM dbo.Query q
left join QueryAnalysis qa ON qa.Id = q.QueryId
--WHERE OriginalCount <> ParsedCount
WHERE name IS NOT NULL
AND name <> 'scratchpad'
AND text not like '%AnyFalse%'
AND text NOT LIKE '%ContributionAmount2%'
";
            public const string Analyze = @"
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
	WHERE (qa.OriginalCount <> qa.ParsedCount AND qa.OriginalCount IS NOT NULL)
	OR (Message LIKE '%xml version%')
    OR (Message NOT LIKE '%xml version%')
ORDER BY q.lastRun desc
";
            public const string Errors = @"
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
	WHERE (qa.OriginalCount <> qa.ParsedCount AND qa.OriginalCount IS NOT NULL)
	OR (Message LIKE '%xml version%')
    OR (Message NOT LIKE '%xml version%')
ORDER BY q.lastRun desc
";
#endif
    }
}