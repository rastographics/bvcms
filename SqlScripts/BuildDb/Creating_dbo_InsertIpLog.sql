CREATE PROCEDURE [dbo].[InsertIpLog] @ip VARCHAR(50), @id VARCHAR(300)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @tm DATETIME = GETDATE()
	IF EXISTS(SELECT NULL FROM dbo.IpLog WHERE ip = @ip AND id = @id)
	BEGIN
		UPDATE dbo.IpLog SET tm = @tm WHERE ip = @ip AND id = @id
		INSERT dbo.Iplog2 (ip, tm) VALUES(@ip, @tm)
	END
	ELSE
		INSERT dbo.IpLog ( ip , id , tm ) VALUES ( @ip, @id, @tm )
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
