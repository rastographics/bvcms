CREATE TABLE [dbo].[SecurityCodes]
(
[Id] [int] NOT NULL,
[Code] [char] (4) NOT NULL,
[DateUsed] [datetime] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
