CREATE PROCEDURE [dbo].[UpdateSchoolGrade] @pid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-----------------------------------------------------------------
	
	UPDATE dbo.People SET Grade = dbo.SchoolGrade(@pid) WHERE PeopleId = @pid

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
