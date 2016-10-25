
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PurgeAllPeopleInCampus](@cid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		DECLARE pcur CURSOR FOR 
		SELECT PeopleId FROM dbo.People
		WHERE CampusId = @cid
		
		OPEN pcur
		DECLARE @pid INT, @n INT
		SET @n = 0
		FETCH NEXT FROM pcur INTO @pid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXECUTE dbo.PurgePerson	@pid
			FETCH NEXT FROM pcur INTO @pid
		END
		CLOSE pcur
		DEALLOCATE pcur
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
