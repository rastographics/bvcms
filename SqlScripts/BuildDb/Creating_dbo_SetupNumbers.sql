-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetupNumbers]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @UpperLimit INT = 50000;
	IF NOT EXISTS(SELECT NULL FROM dbo.Numbers)
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
IF @@ERROR <> 0 SET NOEXEC ON
GO
