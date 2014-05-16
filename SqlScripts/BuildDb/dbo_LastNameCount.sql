-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[LastNameCount](@last nvarchar(20))
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @r INT
	
	SELECT @r = [count] FROM dbo._LastName WHERE LastName = @last
	RETURN @r

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
