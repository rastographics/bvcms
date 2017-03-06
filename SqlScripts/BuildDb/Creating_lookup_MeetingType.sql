CREATE TABLE [lookup].[MeetingType]
(
[Id] [int] NOT NULL,
[Code] [char] (1) NULL,
[Description] [nvarchar] (10) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
