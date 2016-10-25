CREATE TABLE [dbo].[LabelFormats]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_LabelFormats_Name] DEFAULT (''),
[Size] [int] NOT NULL CONSTRAINT [DF_LabelFormats_Size] DEFAULT ((0)),
[Format] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_LabelFormats_Format] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
