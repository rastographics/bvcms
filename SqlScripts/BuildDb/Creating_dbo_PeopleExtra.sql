CREATE TABLE [dbo].[PeopleExtra]
(
[PeopleId] [int] NOT NULL,
[TransactionTime] [datetime] NOT NULL CONSTRAINT [DF_PeopleExtra_TransactionTime] DEFAULT (((1)/(1))/(1900)),
[Field] [nvarchar] (150) NOT NULL,
[StrValue] [nvarchar] (200) NULL,
[DateValue] [datetime] NULL,
[Data] [nvarchar] (max) NULL,
[IntValue] [int] NULL,
[IntValue2] [int] NULL,
[BitValue] [bit] NULL,
[FieldValue] AS (([Field]+':')+isnull([StrValue],[BitValue])),
[UseAllValues] [bit] NULL,
[Instance] [int] NOT NULL CONSTRAINT [DF_PeopleExtra_Instance] DEFAULT ((1)),
[IsAttributes] [bit] NULL,
[Type] AS (case  when [UseAllValues]=(1) then 'Data' when [IsAttributes]=(1) then 'Attr' else (((case  when [StrValue] IS NOT NULL then 'Code' else '' end+case  when [Data] IS NOT NULL then 'Text' else '' end)+case  when [DateValue] IS NOT NULL then 'Date' else '' end)+case  when [IntValue] IS NOT NULL then 'Int' else '' end)+case  when [BitValue] IS NOT NULL then 'Bit' else '' end end),
[Metadata] [nvarchar] (max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
