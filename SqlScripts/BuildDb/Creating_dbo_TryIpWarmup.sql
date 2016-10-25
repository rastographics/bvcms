CREATE PROCEDURE [dbo].[TryIpWarmup]
AS
BEGIN
	SET NOCOUNT ON
    
	DECLARE @cansend BIT

	DECLARE @dt DATETIME = GETDATE()
	DECLARE @newsince DATETIME = (SELECT DATEADD(HOUR, DATEDIFF(HOUR, 0, @dt), 0))

	DECLARE @ep DATETIME, @since DATETIME, @sentsince INT

	SELECT TOP 1 @ep = epoch ,@sentsince = sentsince ,@since = since FROM dbo.IpWarmup

	IF @ep IS NULL
		INSERT dbo.IpWarmup ( epoch, sentsince, since, totalsent, totaltries ) VALUES  ( @dt, 0, @newsince, 0, 0 )

	DECLARE @day INT = DATEDIFF(HOUR, @ep, @newsince) / 24

	DECLARE @max INT = CASE WHEN @day = 0 THEN 20
                    WHEN @day = 1 THEN 28
                    WHEN @day = 2 THEN 39
                    WHEN @day = 3 THEN 55
                    WHEN @day = 4 THEN 77
                    WHEN @day = 5 THEN 108
                    WHEN @day = 6 THEN 151
                    WHEN @day = 7 THEN 211
                    WHEN @day = 8 THEN 295
                    WHEN @day = 9 THEN 413
                    WHEN @day = 10 THEN 579
                    WHEN @day = 11 THEN 810
                    WHEN @day = 12 THEN 1000
                    WHEN @day = 13 THEN 1587
                    WHEN @day = 14 THEN 2222
                    WHEN @day = 15 THEN 3111
                    WHEN @day = 16 THEN 4356
                    WHEN @day = 17 THEN 6098
                    WHEN @day = 18 THEN 8583
                    WHEN @day = 19 THEN 11953
                    WHEN @day = 20 THEN 16734
                    WHEN @day = 21 THEN 23427
                    WHEN @day = 22 THEN 32798
                    WHEN @day = 23 THEN 45917
                    WHEN @day = 24 THEN 64284
                    ELSE 1000000
               END 

	IF @newsince <> @since -- in a new hour
	BEGIN
		UPDATE dbo.IpWarmup
		SET since = @newsince, sentsince = 1, totalsent = totalsent + 1, totaltries = totaltries + 1
		SET @cansend = 1
    END
	ELSE IF @sentsince < @max -- not yet reached max for this hour
	BEGIN
		UPDATE dbo.IpWarmup
		SET sentsince = sentsince + 1, totalsent = totalsent + 1, totaltries = totaltries + 1
		SET @cansend = 1
	END
	ELSE
	BEGIN
		UPDATE dbo.IpWarmup
		SET totaltries = totaltries + 1
		SET @cansend = 0
	END
    
	RETURN @cansend
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
