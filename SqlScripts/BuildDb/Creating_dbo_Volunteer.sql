CREATE TABLE [dbo].[Volunteer]
(
[PeopleId] [int] NOT NULL,
[StatusId] [int] NULL,
[ProcessedDate] [datetime] NULL,
[Standard] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Standard] DEFAULT ((0)),
[Children] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Children] DEFAULT ((0)),
[Leader] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Leader] DEFAULT ((0)),
[Comments] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MVRStatusId] [int] NULL,
[MVRProcessedDate] [datetime] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
