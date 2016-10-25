CREATE PROCEDURE [dbo].[DropOrgMember](@oid INT, @pid INT)
AS
BEGIN
	DECLARE @tranid INT 
	DECLARE @dropdate DATETIME = GETDATE()
	EXEC @tranid = dbo.DropOrgMemberAddET @oid, @pid, @dropdate
	RETURN @tranid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
