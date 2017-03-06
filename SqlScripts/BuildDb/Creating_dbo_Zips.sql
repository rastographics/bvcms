CREATE TABLE [dbo].[Zips]
(
[ZipCode] [nvarchar] (10) NOT NULL,
[MetroMarginalCode] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
