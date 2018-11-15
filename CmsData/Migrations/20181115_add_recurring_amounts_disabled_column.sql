IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Disabled'
          AND Object_ID = Object_ID(N'dbo.RecurringAmounts'))
BEGIN
    ALTER TABLE dbo.RecurringAmounts
    ADD Disabled BIT NOT NULL DEFAULT 0
END
GO