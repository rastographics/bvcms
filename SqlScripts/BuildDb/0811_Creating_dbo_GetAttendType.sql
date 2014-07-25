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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
