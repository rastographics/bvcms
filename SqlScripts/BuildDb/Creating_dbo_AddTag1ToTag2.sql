-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddTag1ToTag2](@t1 INT, @t2 INT) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT dbo.TagPerson
	        ( Id, PeopleId )
			SELECT @t2, t1.PeopleId
			FROM dbo.TagPerson t1
			WHERE Id = @t1
			AND NOT EXISTS(SELECT NULL FROM dbo.TagPerson t2 WHERE Id = @t2 AND PeopleId = t1.PeopleId)
	RETURN @t2
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
