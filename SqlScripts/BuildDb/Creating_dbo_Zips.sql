CREATE TABLE [dbo].[Zips]
(
[ZipCode] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[MetroMarginalCode] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
