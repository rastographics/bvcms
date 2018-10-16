IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Origin'
          AND Object_ID = Object_ID(N'dbo.Contribution'))
BEGIN
    ALTER TABLE dbo.Contribution
    ADD Origin int not null default(0)
END
GO
