CREATE TABLE [dbo].[Preferences]
(
[UserId] [int] NOT NULL,
[Preference] [nvarchar] (30) NOT NULL,
[Value] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
