CREATE TABLE [dbo].[EmailQueueTo]
(
[Id] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[OrgId] [int] NULL,
[Sent] [datetime] NULL,
[AddEmail] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[guid] [uniqueidentifier] NULL,
[messageid] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[GoerSupportId] [int] NULL,
[Parent1] [int] NULL,
[Parent2] [int] NULL,
[Bounced] [bit] NULL,
[SpamReport] [bit] NULL,
[Blocked] [bit] NULL,
[Expired] [bit] NULL,
[SpamContent] [bit] NULL,
[Invalid] [bit] NULL,
[BouncedAddress] [bit] NULL,
[SpamReporting] [bit] NULL,
[DomainFrom] [varchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
