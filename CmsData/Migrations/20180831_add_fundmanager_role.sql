IF NOT EXISTS(SELECT * FROM dbo.Roles WHERE RoleName='FundManager')
BEGIN
    INSERT INTO dbo.Roles (RoleName, hardwired)
    VALUES ('FundManager', 1)
END
GO
