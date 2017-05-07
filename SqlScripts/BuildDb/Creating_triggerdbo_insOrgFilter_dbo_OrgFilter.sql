CREATE TRIGGER [dbo].[insOrgFilter] 
   ON  [dbo].[OrgFilter]
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;
	IF CONVERT(DATETIME, ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'LastOrgFilterCleanup' AND System = 1), '1/1/1900')) < DATEADD(HOUR, -24, GETDATE())
		EXEC dbo.CleanOrgFilters
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
