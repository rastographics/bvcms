CREATE TABLE [dbo].[RecReg]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[ImgId] [int] NULL,
[IsDocument] [bit] NULL,
[ActiveInAnotherChurch] [bit] NULL,
[ShirtSize] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MedAllergy] [bit] NULL,
[email] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MedicalDescription] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[fname] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[mname] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[coaching] [bit] NULL,
[member] [bit] NULL,
[emcontact] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[emphone] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[doctor] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[docphone] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[insurance] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[policy] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Comments] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Tylenol] [bit] NULL,
[Advil] [bit] NULL,
[Maalox] [bit] NULL,
[Robitussin] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
