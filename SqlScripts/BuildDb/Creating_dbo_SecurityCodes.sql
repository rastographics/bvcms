CREATE TABLE [dbo].[SecurityCodes]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Code] [char] (4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DateUsed] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
