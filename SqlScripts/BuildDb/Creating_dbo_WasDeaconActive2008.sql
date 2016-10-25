-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[WasDeaconActive2008](@pid INT, @dt DATETIME)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @r BIT 

	IF EXISTS(SELECT NULL FROM dbo.OrganizationMembers o WHERE o.OrganizationId = 80063 AND @pid = o.PeopleId) AND @dt > '1/1/2008' AND @dt < '11/1/2008'
		SELECT @r = 0
	ELSE IF EXISTS(SELECT NULL FROM dbo.OrganizationMembers o2 WHERE o2.OrganizationId = 80092 AND o2.PeopleId = @pid)
		SELECT @r = 1
	ELSE
		SELECT @r = 0

	-- Return the result of the function
	RETURN @r

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
