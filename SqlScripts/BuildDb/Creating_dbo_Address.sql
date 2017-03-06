CREATE TABLE [dbo].[Address]
(
[Id] [int] NOT NULL,
[Address] [nvarchar] (50) NULL,
[Address2] [nvarchar] (50) NULL,
[City] [nvarchar] (50) NULL,
[State] [nvarchar] (50) NULL,
[Zip] [nvarchar] (50) NULL,
[BadAddress] [bit] NULL,
[FromDt] [datetime] NULL,
[ToDt] [datetime] NULL,
[Type] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
