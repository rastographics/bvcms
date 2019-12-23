IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'ShowList'
          AND Object_ID = Object_ID(N'dbo.ContributionFund'))
BEGIN
    ALTER TABLE dbo.ContributionFund
    ADD ShowList INT NOT NULL DEFAULT 0
END
GO

UPDATE dbo.ContributionFund
SET ShowList = 1
WHERE OnlineSort < 100
AND ShowList = 0

UPDATE dbo.ContributionFund
SET ShowList = 2
WHERE OnlineSort > 100
AND ShowList = 0

UPDATE dbo.ContributionFund
SET ShowList = 3
WHERE OnlineSort IS NULL
AND ShowList = 0
