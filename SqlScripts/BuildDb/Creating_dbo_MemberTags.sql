CREATE TABLE [dbo].[MemberTags]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (200) NULL,
[OrgId] [int] NULL,
[VolFrequency] [nvarchar] (2) NULL,
[VolStartDate] [datetime] NULL,
[VolEndDate] [datetime] NULL,
[NoCancelWeeks] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
