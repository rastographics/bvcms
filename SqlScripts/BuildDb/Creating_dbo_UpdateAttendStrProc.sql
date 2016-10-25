CREATE PROCEDURE [dbo].[UpdateAttendStrProc]
AS
BEGIN
	DECLARE @orgid INT, @dlgId UNIQUEIDENTIFIER	

	WHILE(1=1)
	BEGIN
		RECEIVE top(1) @orgid = CONVERT(INT, message_body), @dlgId = conversation_handle FROM dbo.UpdateAttendStrQueue
		IF @@ROWCOUNT = 0		
			BREAK;
		IF @orgid IS NOT NULL
			EXEC dbo.UpdateAllAttendStr @orgid
		END CONVERSATION @dlgId WITH CLEANUP
	END	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
