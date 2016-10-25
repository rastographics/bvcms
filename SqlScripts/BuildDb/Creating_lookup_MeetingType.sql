CREATE TABLE [lookup].[MeetingType]
(
[Id] [int] NOT NULL,
[Code] [char] (1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Description] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
