CREATE TABLE [dbo].[MemberTags]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[OrgId] [int] NULL,
[VolFrequency] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[VolStartDate] [datetime] NULL,
[VolEndDate] [datetime] NULL,
[NoCancelWeeks] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
