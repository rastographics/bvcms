ALTER FUNCTION [dbo].[SplitInts] ( @List VARCHAR(MAX) )
RETURNS TABLE
AS
    RETURN
    (
        SELECT DISTINCT CAST([Value] as int) [Value]
        FROM
            STRING_SPLIT(@List, ',')
        WHERE TRY_CONVERT(int, [Value]) IS NOT NULL 
          AND RTRIM(LTRIM([Value])) <> ''
    );
GO
