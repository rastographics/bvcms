
CREATE PROCEDURE [dbo].[TrackClick]
	@hash VARCHAR(50), 
	@link VARCHAR(2000) OUTPUT
AS
BEGIN
	SELECT @link = [Link] FROM dbo.EmailLinks WHERE [Hash] = @hash

	UPDATE dbo.EmailLinks
	SET [Count] = [Count] + 1
	WHERE [Hash] = @hash
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
