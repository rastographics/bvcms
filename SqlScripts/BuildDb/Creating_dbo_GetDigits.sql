CREATE Function [dbo].[GetDigits](@Str nvarchar(500))
Returns nvarchar(500)
AS
Begin
        DECLARE @count int
        DECLARE @intNumbers VARCHAR(1000)
        SET @count = 0
        SET @intNumbers = ''

        WHILE @count <= LEN(@str)
        BEGIN 
            IF SUBSTRING(@str, @count, 1)>='0' and SUBSTRING (@str, @count, 1) <='9'
                BEGIN
                    SET @intNumbers = @intNumbers + SUBSTRING (@str, @count, 1)
                END
            SET @count = @count + 1
        END
        RETURN @intNumbers
End
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
