CREATE FUNCTION [dbo].[IsCardTester] ( @ip VARCHAR(50))
RETURNS BIT
AS
BEGIN
	DECLARE @result BIT = 0
	DECLARE @lookback DATETIME = DATEADD(HOUR, 24, GETDATE())
	DECLARE @cnt INT

	SELECT @cnt = COUNT(*) 
				FROM dbo.IpLog 
				WHERE ip = @ip 
				AND tm > @lookback
	IF(@cnt >= 5)
		SET @result = 1
	ELSE
		IF dbo.IpVelocity(@ip, GETDATE()) < 2500
			SET @result = 1
    
	RETURN @result
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
