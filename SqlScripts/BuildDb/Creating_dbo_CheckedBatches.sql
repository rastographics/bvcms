CREATE TABLE [dbo].[CheckedBatches]
(
[BatchRef] [nvarchar] (50) NOT NULL,
[Checked] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
