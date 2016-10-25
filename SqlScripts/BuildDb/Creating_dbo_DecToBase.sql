CREATE function [dbo].[DecToBase]
(
@val as BigInt,
@base as int
)
returns nvarchar(63)
as
Begin
	If (@val<0) OR (@base < 2) OR (@base> 36) 
		Return Null;
	Declare @answer as nvarchar(63) = '';
	Declare @alldigits as nvarchar(36) = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'
	While @val>0
	Begin
		Set @answer=Substring(@alldigits,@val % @base + 1,1) + @answer;
		Set @val = @val / @base;
	End
	return @answer;
End
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
