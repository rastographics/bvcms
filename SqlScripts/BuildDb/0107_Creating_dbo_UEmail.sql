-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[UEmail] (@pid int)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	declare @email nvarchar(100)
	
	SELECT  @email = EmailAddress
	FROM         dbo.People
	WHERE     PeopleId = @pid

	return @email

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
