IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Notes' AND Object_ID = Object_ID(N'dbo.ContributionFund'))
BEGIN
ALTER TABLE dbo.ContributionFund ADD Notes BIT NOT NULL DEFAULT 0
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Notes' AND Object_ID = Object_ID(N'dbo.Contribution'))
BEGIN
ALTER TABLE dbo.Contribution ADD Notes nvarchar(MAX)
END
GO
