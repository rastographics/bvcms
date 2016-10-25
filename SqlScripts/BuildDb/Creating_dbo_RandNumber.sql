CREATE VIEW [dbo].[RandNumber]
AS
SELECT RAND() as RandNumber
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
