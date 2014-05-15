-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetupNumbers]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UpperLimit INT = 10000;
	IF(SELECT COUNT(*) FROM dbo.Numbers) = 0
	BEGIN
		WITH n AS
		(
		    SELECT
		        rn = ROW_NUMBER() OVER
		        (ORDER BY s1.[object_id])
		    FROM sys.objects AS s1
		    CROSS JOIN sys.objects AS s2
		    CROSS JOIN sys.objects AS s3
		)
		INSERT dbo.Numbers ( Number )
		(SELECT rn - 1
		 FROM n
		 WHERE rn <= @UpperLimit + 1)
	END
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
