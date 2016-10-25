-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[UName] (@pid int)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	declare @name nvarchar(100)
	
	SELECT  @name = (case when [Nickname]<>'' then [nickname] else [FirstName] end) + ' ' + [LastName]
	FROM         dbo.People
	WHERE     PeopleId = @pid

	return @name

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
