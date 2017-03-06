CREATE TABLE [dbo].[RecReg]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[PeopleId] [int] NULL,
[ImgId] [int] NULL,
[IsDocument] [bit] NULL,
[ActiveInAnotherChurch] [bit] NULL,
[ShirtSize] [nvarchar] (50) NULL,
[MedAllergy] [bit] NULL,
[email] [nvarchar] (80) NULL,
[MedicalDescription] [nvarchar] (1000) NULL,
[fname] [nvarchar] (80) NULL,
[mname] [nvarchar] (80) NULL,
[coaching] [bit] NULL,
[member] [bit] NULL,
[emcontact] [nvarchar] (100) NULL,
[emphone] [nvarchar] (50) NULL,
[doctor] [nvarchar] (100) NULL,
[docphone] [nvarchar] (50) NULL,
[insurance] [nvarchar] (100) NULL,
[policy] [nvarchar] (100) NULL,
[Comments] [nvarchar] (max) NULL,
[Tylenol] [bit] NULL,
[Advil] [bit] NULL,
[Maalox] [bit] NULL,
[Robitussin] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
