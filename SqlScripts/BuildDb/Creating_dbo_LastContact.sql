-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[LastContact](@pid INT)
RETURNS DATETIME
AS
BEGIN
	DECLARE @dt DATETIME

	SELECT @dt = MAX(c.ContactDate) FROM dbo.Contact c
	JOIN dbo.Contactees ce ON c.ContactId = ce.ContactId
	WHERE ce.PeopleId = @pid
	IF @dt IS NULL
		SELECT @dt = DATEADD(DAY,-5000,GETDATE())

	RETURN @dt

END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
