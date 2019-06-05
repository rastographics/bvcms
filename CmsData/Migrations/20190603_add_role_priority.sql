IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Priority'
          AND Object_ID = Object_ID(N'dbo.Roles'))
BEGIN
    EXEC dbo.sp_executesql N'
        ALTER TABLE dbo.Roles
        ADD Priority INT NULL;'
    EXEC dbo.sp_executesql N'
        UPDATE dbo.Roles
        SET Priority = RoleId;'
END
GO
