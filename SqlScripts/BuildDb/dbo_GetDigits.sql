CREATE Function [dbo].[GetDigits](@Str nvarchar(500))
Returns nvarchar(500)
AS
Begin
	While PatIndex('%[^0-9]%', @Str) > 0
	     Set @Str = Stuff(@Str, PatIndex('%[^0-9]%', @Str), 1, '')
	RETURN @Str 
End
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
