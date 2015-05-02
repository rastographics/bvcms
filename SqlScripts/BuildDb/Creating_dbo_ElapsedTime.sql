-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ElapsedTime](@start DATETIME, @end DATETIME)
RETURNS varchar(25)
AS
BEGIN
	DECLARE @ret VARCHAR(25)
	SELECT @ret = CAST(
        (CAST(CAST(@end AS FLOAT) - CAST(@start AS FLOAT) AS INT) * 24) /* hours over 24 */
        + DATEPART(hh, @end - @start) /* hours */
        AS VARCHAR(10))
    + ':' + RIGHT('0' + CAST(DATEPART(mi, @end - @start) AS VARCHAR(2)), 2) /* minutes */
    + ':' + RIGHT('0' + CAST(DATEPART(ss, @end - @start) AS VARCHAR(2)), 2) /* seconds */	
	RETURN @ret
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
