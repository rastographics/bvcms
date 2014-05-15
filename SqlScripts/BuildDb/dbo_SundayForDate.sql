create function dbo.SundayForDate(@dt DATETIME) 
returns		datetime
as
begin
	declare	 @stdt	datetime
	declare	 @fdt	datetime
	select @fdt = convert(datetime,-53690+((1+5)%7))
	select @stdt = dateadd(dd,(datediff(dd,@fdt,@dt)/7)*7,@fdt)
	return @stdt
	
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
