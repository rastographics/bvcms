-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetSecurityCode]()
RETURNS CHAR(3)
AS
BEGIN
	DECLARE @code CHAR(3)
	EXEC NextSecurityCode @code OUTPUT
	RETURN @code
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
