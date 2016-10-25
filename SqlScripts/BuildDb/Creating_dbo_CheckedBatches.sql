CREATE TABLE [dbo].[CheckedBatches]
(
[BatchRef] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
[Checked] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
