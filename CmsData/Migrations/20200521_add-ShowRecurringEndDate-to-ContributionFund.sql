IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'ShowRecurringEndDate'
          AND Object_ID = Object_ID(N'dbo.ContributionFund'))
BEGIN
    ALTER TABLE dbo.ContributionFund
    ADD ShowRecurringEndDate BIT NOT NULL CONSTRAINT DF_ContributionFund_ShowRecurringEndDate DEFAULT (0)
END
GO