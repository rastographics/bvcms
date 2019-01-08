IF NOT EXISTS(SELECT * FROM dbo.Roles WHERE RoleName='ViewVolunteerApplication')
BEGIN
    INSERT INTO dbo.Roles (RoleName, hardwired)
    VALUES ('ViewVolunteerApplication', 1)
END
GO