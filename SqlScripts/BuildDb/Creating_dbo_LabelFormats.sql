CREATE TABLE [dbo].[LabelFormats]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (30) NOT NULL CONSTRAINT [DF_LabelFormats_Name] DEFAULT (''),
[Size] [int] NOT NULL CONSTRAINT [DF_LabelFormats_Size] DEFAULT ((0)),
[Format] [text] NOT NULL CONSTRAINT [DF_LabelFormats_Format] DEFAULT ('')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
