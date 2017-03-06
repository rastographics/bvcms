CREATE TABLE [dbo].[words]
(
[word] [nvarchar] (20) NOT NULL,
[n] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
