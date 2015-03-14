-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[WeekNumber](@dt DATETIME)
RETURNS INT
AS
BEGIN

DECLARE @wkn INT 

SELECT @wkn = DATEPART(yy, @dt) * 100 + DATEPART(ww, @dt) - 1

	-- Return the result of the function
	RETURN @wkn

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
