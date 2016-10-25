CREATE TABLE [dbo].[PeopleExtra]
(
[PeopleId] [int] NOT NULL,
[TransactionTime] [datetime] NOT NULL CONSTRAINT [DF_PeopleExtra_TransactionTime] DEFAULT (((1)/(1))/(1900)),
[Field] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[StrValue] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DateValue] [datetime] NULL,
[Data] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IntValue] [int] NULL,
[IntValue2] [int] NULL,
[BitValue] [bit] NULL,
[FieldValue] AS (([Field]+':')+isnull([StrValue],[BitValue])),
[UseAllValues] [bit] NULL,
[Instance] [int] NOT NULL CONSTRAINT [DF_PeopleExtra_Instance] DEFAULT ((1)),
[Type] AS (case  when [UseAllValues]=(1) then 'Data' else (((case  when [StrValue] IS NOT NULL then 'Code' else '' end+case  when [Data] IS NOT NULL then 'Text' else '' end)+case  when [DateValue] IS NOT NULL then 'Date' else '' end)+case  when [IntValue] IS NOT NULL then 'Int' else '' end)+case  when [BitValue] IS NOT NULL then 'Bit' else '' end end)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
