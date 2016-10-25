CREATE TABLE [dbo].[MeetingExtra]
(
[MeetingId] [int] NOT NULL,
[Field] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Data] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DataType] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StrValue] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DateValue] [datetime] NULL,
[IntValue] [int] NULL,
[BitValue] [bit] NULL,
[TransactionTime] [datetime] NULL,
[UseAllValues] [bit] NULL,
[Type] AS (((((case  when [UseAllValues]=(1) then 'Data' else '' end+case  when [StrValue] IS NOT NULL then 'Code' else '' end)+case  when [Data] IS NOT NULL then 'Text' else '' end)+case  when [DateValue] IS NOT NULL then 'Date' else '' end)+case  when [IntValue] IS NOT NULL then 'Int' else '' end)+case  when [BitValue] IS NOT NULL then 'Bit' else '' end)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
