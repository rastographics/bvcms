-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PurgeAllPeople]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		DECLARE pcur CURSOR FOR SELECT PeopleId FROM dbo.People
		OPEN pcur
		DECLARE @pid INT, @n INT
		SET @n = 0
		FETCH NEXT FROM pcur INTO @pid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE dbo.PurgePerson	@pid
			SET @n = @n + 1
			IF (@n % 50) = 0
				RAISERROR ('%d', 0, 1, @n) WITH NOWAIT
			IF (@n % 3) = 0
			BEGIN
				FETCH NEXT FROM pcur INTO @pid
				SET @n = @n + 1
				IF (@n % 50) = 0
					RAISERROR ('%d', 0, 1, @n) WITH NOWAIT
			END
			FETCH NEXT FROM pcur INTO @pid
		END
		CLOSE pcur
		DEALLOCATE pcur
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
