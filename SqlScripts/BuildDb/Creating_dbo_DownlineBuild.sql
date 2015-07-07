-- ================================================
CREATE PROCEDURE [dbo].[DownlineBuild]
AS
BEGIN
	SET NOCOUNT ON;

	EXEC dbo.DownlineCreateCategoryConfigContent

	TRUNCATE TABLE dbo.Downline
	TRUNCATE TABLE dbo.DownlineData

BEGIN TRANSACTION;

BEGIN TRY
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
END TRY
BEGIN CATCH
    SELECT 
        ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity
        ,ERROR_STATE() AS ErrorState
        ,ERROR_PROCEDURE() AS ErrorProcedure
        ,ERROR_LINE() AS ErrorLine
        ,ERROR_MESSAGE() AS ErrorMessage;

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;


	


END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
