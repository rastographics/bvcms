CREATE FUNCTION [dbo].[GenRanges](@amts VARCHAR(1000))
RETURNS @ts TABLE
(amt INT, MinAmt INT, MaxAmt INT)
AS
BEGIN

	DECLARE @t TABLE (rn INT IDENTITY(1,1), amt INT)

	INSERT @t (amt)
	SELECT id FROM dbo.CsvTable(@amts)

	;WITH CTE AS
	(
	      SELECT rn, amt, CAST(1 AS INT) AS MinAmt,
	             amt AS MaxAmt,1 AS INIT
	      FROM @t
	      UNION ALL
	      SELECT a.rn,a.amt,c.MaxAmt+1,
	             c.MaxAmt + a.rn,INIT+1
	      FROM CTE c
	      JOIN @t a ON c.rn+1 = a.rn
	)
	INSERT @ts
	SELECT      amt,
	            CASE WHEN amt = 0 THEN 0 ELSE MAX(MinAmt) END MinAmt,
	            CASE WHEN amt = 0 THEN 0 ELSE MAX(MaxAmt) END MaxAmt
	FROM CTE
	GROUP BY rn, amt
	ORDER BY rn

	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
