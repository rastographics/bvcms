CREATE FUNCTION [dbo].[NextBirthday](@pid int)
RETURNS datetime
AS
	BEGIN
	
	
	  DECLARE
		@today DATETIME,
		@date datetime, 
		@m int,
		@d int,
		@y int
		
SELECT @today = CONVERT(datetime, CONVERT(nvarchar, GETDATE(), 112))
select @date = null
select @m = BirthMonth, @d = BirthDay from dbo.People where @pid = PeopleId
if @m is null or @d is null
	return @date
select @y = DATEPART(year, @today) 
select @date = dateadd(mm,(@y-1900)* 12 + @m - 1,0) + (@d-1) 
if @date < @today
	select @date = dateadd(yy, 1, @date)
RETURN @date
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
