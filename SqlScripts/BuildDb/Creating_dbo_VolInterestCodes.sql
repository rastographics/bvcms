CREATE TABLE [dbo].[VolInterestCodes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Description] [nvarchar] (180) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Code] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Org] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
