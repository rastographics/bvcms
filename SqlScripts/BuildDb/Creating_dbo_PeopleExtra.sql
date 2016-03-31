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
[Type] AS (((((case when [UseAllValues]=(1) then 'Data' else '' end+case when [StrValue] IS NOT NULL then 'Code' else '' end)+case when [Data] IS NOT NULL then 'Text' else '' end)+case when [DateValue] IS NOT NULL then 'Date' else '' end)+case when [IntValue] IS NOT NULL then 'Int' else '' end)+case when [BitValue] IS NOT NULL then 'Bit' else '' end)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
