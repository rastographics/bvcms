CREATE PROC [dbo].[ResetAllProgramDivision]
AS
BEGIN
	UPDATE dbo.Organizations SET DivisionId = NULL
	UPDATE dbo.Division SET ProgId = NULL
	DELETE dbo.DivOrg
	DELETE dbo.ProgDiv
	DELETE dbo.Division
	DELETE dbo.Program
	SET IDENTITY_INSERT dbo.Program ON
	INSERT dbo.Program ( Id, Name ) VALUES  ( 1, 'First Program' )
	SET IDENTITY_INSERT dbo.Program OFF
	SET IDENTITY_INSERT dbo.Division ON
	INSERT dbo.Division ( Id, Name, ProgId ) VALUES  ( 1, 'First Division', 1 )
	SET IDENTITY_INSERT dbo.Division OFF
	UPDATE dbo.Organizations SET DivisionId = 1
	INSERT dbo.DivOrg ( DivId, OrgId ) SELECT 1, OrganizationId FROM dbo.Organizations
	INSERT dbo.ProgDiv ( ProgId, DivId ) VALUES (1,1)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
