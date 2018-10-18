IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'PushPayLog')
CREATE TABLE [dbo].PushPayLog(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchPage] [int] NULL,
	[BatchDate] [datetime] NULL,
	[BatchKey] [nvarchar](100) NULL,
	[MerchantKey] [nvarchar](100) NULL,
	[TransactionId] [nvarchar](100) NULL,
	[ImportDate] [datetime] NULL
	)

GO

