CREATE PROCEDURE [dbo].[TrackOpen](@guid UNIQUEIDENTIFIER)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @id INT, @pid INT
	SELECT @id = id, @pid = PeopleId FROM dbo.EmailQueueTo WHERE guid = @guid
	INSERT dbo.EmailResponses (EmailQueueId, PeopleId, [Type], Dt) VALUES (@id, @pid, 'o', GETDATE())
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
