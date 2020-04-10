ALTER FUNCTION [dbo].[SplitInts] ( @List VARCHAR(MAX) )
RETURNS TABLE
AS
    RETURN
    (
        SELECT DISTINCT CAST([Value] as int) [Value]
        FROM
            STRING_SPLIT(@List, ',')
        WHERE
        RTRIM(LTRIM([Value])) <> ''
    );
GO