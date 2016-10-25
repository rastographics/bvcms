CREATE TABLE [dbo].[ActivityLog]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[ActivityDate] [datetime] NULL,
[UserId] [int] NULL,
[Activity] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PageUrl] [nvarchar] (410) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Machine] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[OrgId] [int] NULL,
[PeopleId] [int] NULL,
[DatumId] [int] NULL,
[ClientIp] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
