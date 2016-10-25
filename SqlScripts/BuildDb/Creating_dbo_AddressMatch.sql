CREATE FUNCTION [dbo].[AddressMatch]
	(
	@var1 nvarchar ,@var2 nvarchar
	)
RETURNS int
AS
	BEGIN

-----------------------------------------------------------

-- Function Address --

-- • Matches street name

-----------------------------------------------------------

BEGIN

DECLARE

@a nvarchar(100), -- street name in var1

@b nvarchar(100), -- street name in var2

@c nvarchar(100), -- street name in var1

@d nvarchar(100), -- street name in var2

@e nvarchar(100), -- street name in var1

@f nvarchar(100), -- street name in var2

@i char(1),

@j char(1),

@k char(1),

@var1Tokens int,

@var2Tokens int,

@var1Plus int,

@var2Plus int

set @var1Plus = 0

set @var2Plus = 0

select @var1tokens = TokenID from dbo.Split(@var1, ' ')

select @var2tokens = TokenID from dbo.Split(@var2, ' ')

set @k = '0'

select @a = Value from dbo.Split(@var1, ' ') where TokenID = 1

select @b = Value from dbo.Split(@var2, ' ') where TokenID = 1

if @a = @b set @k = '1'

select @c = value from dbo.Split(@var1, ' ') where TokenID = 2

select @d = Value from dbo.Split(@var2, ' ') where TokenID = 2

set @i = Convert(nvarchar, difference(@c, @d))

if Convert(int, @i) < 3 begin

if @var1tokens > 3 begin

-- select @c = @c + ' ' + value from dbo.Split(@var1, ' ') where TokenID = 3

select @c = value from dbo.Split(@var1, ' ') where TokenID = 3

set @var1Plus = 1

end

if @var2tokens > 3 begin

-- select @d = @d + ' ' + Value from dbo.Split(@var2, ' ') where TokenID = 3

select @d = value from dbo.Split(@var1, ' ') where TokenID = 3

set @var2Plus = 1

end

set @i = Convert(nvarchar, difference(@c, @d))

end

select @e = Value from dbo.Split(@var1, ' ') where TokenID = 3 + @var1Plus

select @f = Value from dbo.Split(@var2, ' ') where TokenID = 3 + @var2Plus

if @e is null or @f is null or @e = '' or @f = ''

set @j = '4'

else

set @j = Convert(nvarchar, difference(@e, @f))

RETURN Convert(int, (@k + @i + @j))

END
	END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
