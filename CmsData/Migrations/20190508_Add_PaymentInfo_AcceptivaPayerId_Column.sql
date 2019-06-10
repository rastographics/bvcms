IF NOT EXISTS(SELECT 1 FROM sys.columns
          WHERE Name = N'AcceptivaPayerId'
          AND Object_ID = Object_ID(N'dbo.PaymentInfo'))
BEGIN
    ALTER TABLE dbo.PaymentInfo
    ADD AcceptivaPayerId NVARCHAR(50) NULL
END
GO
