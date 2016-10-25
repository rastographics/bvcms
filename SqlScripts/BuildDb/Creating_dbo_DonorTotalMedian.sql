CREATE FUNCTION [dbo].[DonorTotalMedian](@t DonorTotalsTable READONLY, @attr INT, @threshold MONEY)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret =
		(SELECT TOP 1 tot FROM 
			(SELECT TOP 50 PERCENT tot FROM @t 
			 WHERE (attr = @attr OR @attr IS NULL)
			 AND tot >= @threshold 
			 ORDER BY tot) A 
		 ORDER BY tot DESC
		) +
	    (SELECT TOP 1 tot FROM 
			(SELECT TOP 50 PERCENT tot FROM @t 
			 WHERE (attr = @attr OR @attr IS NULL)
			 AND tot >= @threshold 
			 ORDER BY tot DESC) A 
		 ORDER BY tot
		) / 2
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
