CREATE TABLE [dbo].[Volunteer]
(
[PeopleId] [int] NOT NULL,
[StatusId] [int] NULL,
[ProcessedDate] [datetime] NULL,
[Standard] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Standard] DEFAULT ((0)),
[Children] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Children] DEFAULT ((0)),
[Leader] [bit] NOT NULL CONSTRAINT [DF_Volunteer_Leader] DEFAULT ((0)),
[Comments] [nvarchar] (max) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
