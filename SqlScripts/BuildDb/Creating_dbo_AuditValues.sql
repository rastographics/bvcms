CREATE TABLE [dbo].[AuditValues]
(
[AuditId] [int] NOT NULL,
[MemberName] [nvarchar] (50) NOT NULL,
[OldValue] [nvarchar] (max) NULL,
[NewValue] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
