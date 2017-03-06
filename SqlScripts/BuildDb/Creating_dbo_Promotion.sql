CREATE TABLE [dbo].[Promotion]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[FromDivId] [int] NULL,
[ToDivId] [int] NULL,
[Description] [nvarchar] (200) NULL,
[Sort] [nvarchar] (10) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
