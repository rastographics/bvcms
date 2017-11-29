CREATE PROCEDURE [dbo].[InsertRogueIp] @ip VARCHAR(50), @db VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF @ip IS NOT NULL AND NOT EXISTS(SELECT NULL FROM BlogData.dbo.RogueIps WHERE @ip = ip )
		INSERT BlogData.dbo.RogueIps ( ip, db, tm ) VALUES ( @ip, @db, GETDATE() )
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
