CREATE PROCEDURE [dbo].[InsertIpLog] @ip VARCHAR(50), @id VARCHAR(300)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT NULL FROM dbo.IpLog WHERE ip = @ip AND id = @id)
		UPDATE dbo.IpLog SET tm = GETDATE() WHERE ip = @ip AND id = @id
	ELSE
		INSERT dbo.IpLog ( ip , id , tm ) VALUES ( @ip, @id, GETDATE() )
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
