CREATE TABLE [lookup].[ContactReason]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (20) NOT NULL,
[Description] [nvarchar] (100) NOT NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
