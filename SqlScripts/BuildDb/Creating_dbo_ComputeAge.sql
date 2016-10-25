CREATE FUNCTION [dbo].[ComputeAge] (@m int, @d int, @y int )
RETURNS int
AS
	BEGIN
	
	  DECLARE
		@v_return int, 
		@v_end_date datetime,
		@p_deceased_date datetime,
		@p_drop_code_id int
		
         SET @v_return = NULL

         IF @y IS NOT NULL 
            BEGIN

               SET @v_return = datepart(YEAR, GETDATE()) - @y

               IF isnull(@m, 1) > datepart(MONTH, GETDATE())
                  SET @v_return = @v_return - 1
               ELSE 
                  IF isnull(@m, 1) = datepart(MONTH, GETDATE()) AND isnull(@d, 1) > datepart(DAY, GETDATE())
                     SET @v_return = @v_return - 1

            END

	RETURN @v_return
	END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
