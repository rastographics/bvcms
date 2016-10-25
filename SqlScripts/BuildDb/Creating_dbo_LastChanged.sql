CREATE FUNCTION [dbo].[LastChanged](@pid int, @field nvarchar(20))
RETURNS DATETIME
AS
BEGIN
	DECLARE @dt DATETIME
	SELECT TOP 1 @dt = Created FROM ChangeLog
	WHERE PeopleId = @pid
	AND Data LIKE ('%<td>' + @field + '</td>%')
	ORDER BY Created DESC
	RETURN @dt
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
