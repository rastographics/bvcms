CREATE PROC [dbo].FetchOrCreateContactReasonId(@reason VARCHAR(100), @rid INT OUTPUT)
AS
BEGIN
	SELECT @rid = Id FROM lookup.ContactReason WHERE [Description] = @reason
	IF @rid IS NULL
	BEGIN
		SELECT @rid = MAX(id) + 10 FROM lookup.ContactReason
		INSERT lookup.ContactReason (Id, Code, [Description]) VALUES (@rid,LEFT(@reason,20),@reason)
	END  
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
