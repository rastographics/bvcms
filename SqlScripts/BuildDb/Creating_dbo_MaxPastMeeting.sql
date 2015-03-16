CREATE FUNCTION [dbo].[MaxPastMeeting] ()
RETURNS DATETIME
AS
BEGIN
    DECLARE @maxcurrentmeeting DATETIME,
			@tzoffset INT,
			@earlycheckinhours INT = 10 -- to include future meetings
			
	SELECT @tzoffset = CONVERT(INT, Setting) FROM dbo.Setting WHERE Id = 'TZOffset'
	SELECT @maxcurrentmeeting = DATEADD(hh ,ISNULL(@tzoffset,0) + @earlycheckinhours, GETDATE())

	RETURN @maxcurrentmeeting
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
