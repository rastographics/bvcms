CREATE FUNCTION [dbo].[IpVelocity](@ip VARCHAR(50), @start DATETIME)
RETURNS INT
AS
BEGIN
	DECLARE @ret FLOAT
	DECLARE @cnt INT, @avg FLOAT

	;WITH tms AS (
	SELECT 
			ip, tm, ISNULL(LAG(tm) OVER (ORDER BY tm), 0) prevtm
			FROM dbo.IpLog2
			WHERE ip = @ip
			AND tm >= @start
	),
	topx AS (
		SELECT TOP 10 ip, tm, tms.prevtm, DATEDIFF_BIG(MILLISECOND, prevtm, tm) elt
		FROM tms
		ORDER BY tm DESC
	)
	SELECT @ret = AVG(elt), @cnt = COUNT(*)  FROM topx
	GROUP BY ip

	IF @cnt <= 4
		SET @ret = NULL

	RETURN @ret


END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
