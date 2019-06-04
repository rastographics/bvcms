IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Priority'
          AND Object_ID = Object_ID(N'dbo.Roles'))
BEGIN
    ALTER TABLE dbo.Roles
    ADD Priority INT NULL

    UPDATE dbo.Roles
    SET Priority = RoleId
END
GO
