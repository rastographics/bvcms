CREATE FUNCTION [dbo].[Age](@pid int)
RETURNS int
AS
	BEGIN
	
	  DECLARE
		@v_return int, 
		@v_end_date datetime,
		@m int,
		@d int,
		@y int,
		@p_deceased_date datetime,
		@p_drop_code_id int
		
select @m = BirthMonth, @d = BirthDay, @y = BirthYear, @p_deceased_date = DeceasedDate, @p_drop_code_id = DropCodeId from dbo.People where @pid = PeopleId


         SET @v_return = NULL

         IF @y IS NOT NULL AND NOT (@p_deceased_date IS NULL AND isnull(@p_drop_code_id, 0) = 30)
            /* 30=Deceased*/
            BEGIN

               SET @v_end_date = isnull(@p_deceased_date, getdate())

               SET @v_return = datepart(YEAR, @v_end_date) - @y

               IF isnull(@m, 1) > datepart(MONTH, @v_end_date)
                  SET @v_return = @v_return - 1
               ELSE 
                  IF isnull(@m, 1) = datepart(MONTH, @v_end_date) AND isnull(@d, 1) > datepart(DAY, @v_end_date)
                     SET @v_return = @v_return - 1

            END

	RETURN @v_return
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
