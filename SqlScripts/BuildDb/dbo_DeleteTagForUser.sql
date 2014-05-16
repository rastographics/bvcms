CREATE PROCEDURE [dbo].[DeleteTagForUser](@tag nvarchar, @user nvarchar)
AS
	/* SET NOCOUNT ON */
	declare @id int
	select @id = id from tag where name = @tag and @user = owner
	
	delete from tagperson where id = @id
	delete from tagshare where tagid = @id
	delete from tag where id = @id
	
	RETURN
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
