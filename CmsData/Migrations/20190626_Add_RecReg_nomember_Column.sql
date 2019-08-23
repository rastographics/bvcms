IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'nomember'
          AND Object_ID = Object_ID(N'dbo.RecReg'))
BEGIN
    ALTER TABLE [dbo].[RecReg]
    ADD [nomember] BIT NULL
END
GO