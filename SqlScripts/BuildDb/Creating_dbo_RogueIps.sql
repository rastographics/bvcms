CREATE VIEW [dbo].[RogueIps] AS 
	SELECT * FROM BlogData.dbo.RogueIps
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
