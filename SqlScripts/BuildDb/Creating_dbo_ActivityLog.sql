CREATE TABLE [dbo].[ActivityLog]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[ActivityDate] [datetime] NULL,
[UserId] [int] NULL,
[Activity] [nvarchar] (200) NULL,
[PageUrl] [nvarchar] (410) NULL,
[Machine] [nvarchar] (50) NULL,
[OrgId] [int] NULL,
[PeopleId] [int] NULL,
[DatumId] [int] NULL,
[ClientIp] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
