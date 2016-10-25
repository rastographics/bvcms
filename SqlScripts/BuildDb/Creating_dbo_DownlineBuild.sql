-- ================================================
CREATE PROCEDURE [dbo].[DownlineBuild]
AS
BEGIN
	SET NOCOUNT ON;

	EXEC dbo.DownlineCreateCategoryConfigContent

	TRUNCATE TABLE dbo.Downline
	TRUNCATE TABLE dbo.DownlineLeaders

	ALTER INDEX [IX_Downline] ON [dbo].[Downline] DISABLE
	ALTER INDEX [IXDownlineCatLeaderDisc] ON [dbo].[Downline] DISABLE

	DECLARE @categoryid INT

	DECLARE @n INT = 1, @maxrows INT
	SELECT @maxrows = COUNT(*) FROM dbo.DownlineCategories(NULL)
	
	WHILE @n <= @maxrows
	BEGIN

		SELECT @categoryid = Id FROM dbo.DownlineCategories(NULL) WHERE rownum = @n
		RAISERROR ('Calling DownlineBuildCategoryHistory  %i', 0, 1, @categoryid) WITH NOWAIT
		EXEC dbo.DownlineBuildCategoryHistory @categoryid
		
	    SELECT @n = @n + 1
	END
	
	ALTER INDEX [IX_Downline] ON [dbo].[Downline] REBUILD
	ALTER INDEX [IXDownlineCatLeaderDisc] ON [dbo].[Downline] REBUILD

	SET @n = 1

	WHILE @n <= @maxrows
	BEGIN

		SELECT @categoryid = Id FROM dbo.DownlineCategories(NULL) WHERE rownum = @n
		RAISERROR ('Calling DownlineBuildSummary  %i', 0, 1, @categoryid) WITH NOWAIT
		EXEC dbo.DownlineBuildSummary @categoryid
	    SELECT @n = @n + 1
	END

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
