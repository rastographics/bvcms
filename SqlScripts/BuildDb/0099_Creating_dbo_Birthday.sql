CREATE FUNCTION [dbo].[Birthday](@pid int)
RETURNS DATETIME
AS
BEGIN
	
	DECLARE
		@dt DATETIME, 
		@m int,
		@d int,
		@y int
    SET @dt = NULL
		
	select @m = BirthMonth, @d = BirthDay, @y = BirthYear from dbo.People where @pid = PeopleId
	IF NOT (@m IS NULL OR @y IS NULL OR @d IS NULL)
	    SET @dt = dateadd(month,((@y-1900)*12)+@m-1,@d-1)
	RETURN @dt
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
