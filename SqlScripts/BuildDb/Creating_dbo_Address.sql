CREATE TABLE [dbo].[Address]
(
[Id] [int] NOT NULL,
[Address] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Address2] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[City] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[State] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Zip] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[BadAddress] [bit] NULL,
[FromDt] [datetime] NULL,
[ToDt] [datetime] NULL,
[Type] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
