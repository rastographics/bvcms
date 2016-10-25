CREATE FUNCTION [dbo].[MemberDesc](@id int) 
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @ret nvarchar(100)
	SELECT @ret =  Description FROM lookup.MemberType WHERE id = @id
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
