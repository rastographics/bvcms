CREATE TABLE [dbo].[Volunteer]
(
[PeopleId] [int] NOT NULL,
[StatusId] [int] NULL,
[ProcessedDate] [datetime] NULL,
[Standard] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Standard] DEFAULT ((0)),
[Children] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Children] DEFAULT ((0)),
[Leader] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Leader] DEFAULT ((0)),
[Comments] [nvarchar] (max) NULL,
[MVRStatusId] [int] NULL,
[MVRProcessedDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
