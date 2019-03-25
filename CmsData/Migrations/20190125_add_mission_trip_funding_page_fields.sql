
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'TripFundingPagesEnable'
          AND Object_ID = Object_ID(N'dbo.Organizations'))
BEGIN
    ALTER TABLE dbo.Organizations
    ADD TripFundingPagesEnable BIT NOT NULL DEFAULT 0,
    TripFundingPagesPublic BIT NOT NULL DEFAULT 0,
    TripFundingPagesShowAmounts BIT NOT NULL DEFAULT 0
END
GO
