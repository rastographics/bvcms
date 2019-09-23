CREATE FUNCTION [dbo].[SplitFundIds] ( @List VARCHAR(MAX) )
RETURNS TABLE
AS
    RETURN
    (
        SELECT DISTINCT
			[Numbers].[value]
		FROM
			STRING_SPLIT (@List , ',') [Numbers]
    );

GO