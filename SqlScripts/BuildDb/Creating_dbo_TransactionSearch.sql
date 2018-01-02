CREATE FUNCTION [dbo].[TransactionSearch]
(
	@name NVARCHAR(150),
	@minamt DECIMAL,
	@maxamt DECIMAL,
	@mindt DATETIME,
	@maxdt DATETIME,
	@description NVARCHAR(100),
	@testtransactions BIT,
	@approvedtransactions BIT,
	@nocoupons BIT,
	@finance BIT,
	@usebatchdates BIT
)
RETURNS @t TABLE (Id INT)
AS
BEGIN
	DECLARE @first NVARCHAR(50)
	DECLARE @last NVARCHAR(100)
	DECLARE @nameid INT = TRY_PARSE(@name AS INT)
	IF @maxdt IS NULL AND @mindt IS NOT NULL
		SET @maxdt = @mindt
	IF @maxdt IS NOT NULL
		SET @maxdt = DATEADD(HOUR, 24, @maxdt)
	SELECT @first = [First], @last = [Last] FROM dbo.FirstLast(@name)

	INSERT @t (Id)
	SELECT Id
	FROM dbo.[Transaction] t
	WHERE (Amt >= @minamt OR @minamt IS NULL)
	AND (Amt <= @maxamt OR @maxamt IS NULL)
	AND (@description IS NULL OR t.Description LIKE '%' + @description + '%')
	AND (@nameid > 0 OR ISNULL(t.testing, 0) = ISNULL(@testtransactions, 0))
	AND (@approvedtransactions = t.moneytran OR ISNULL(@approvedtransactions, 0) = 0)
	AND ((@nocoupons = 1 AND t.TransactionId LIKE '%Coupon%') OR ISNULL(@nocoupons, 0) = 0)
	AND (ISNULL(t.financeonly, 0) = 0 OR @finance = 1)
	AND (@name IS NULL OR (
			ISNULL(@name, '') <> '0' OR t.OriginalId = @nameid)
			AND (ISNULL(@name, '') = '0' OR (((t.Last LIKE @last + '%' OR t.Last LIKE @name + '%')
						AND (ISNULL(@first, '') = '' OR t.First LIKE @first + '%' OR t.Last LIKE @name + '%'))
					OR t.batchref = @name OR t.TransactionId = @name OR t.OriginalId = @nameid OR t.Id = @nameid)
					)
			)
	AND (ISNULL(@usebatchdates, 0) = 0 OR (
			(t.batch >= @mindt OR @mindt IS NULL)
			AND t.batch <= @maxdt OR @maxdt IS NULL)
			AND t.moneytran = 1
			)
	AND (ISNULL(@usebatchdates, 0) = 1 OR (
			(t.TransactionDate >= @mindt OR @mindt IS NULL)
			AND t.TransactionDate <= @maxdt OR @maxdt IS NULL)
			)
	RETURN
END		
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
