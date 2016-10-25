CREATE TABLE [dbo].[Promotion]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[FromDivId] [int] NULL,
[ToDivId] [int] NULL,
[Description] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Sort] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
