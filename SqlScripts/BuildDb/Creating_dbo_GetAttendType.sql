CREATE FUNCTION [dbo].[GetAttendType]
    (
      @attended BIT,
      @membertypeid INT,
      @group BIT
    )
RETURNS INT
AS 
BEGIN
	IF @group = 1
		RETURN 90 -- group
	IF @attended <> 1
		RETURN NULL
	DECLARE @at INT
	SELECT @at = AttendanceTypeId 
	FROM lookup.MemberType
	WHERE Id = @membertypeid
	RETURN @at
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
