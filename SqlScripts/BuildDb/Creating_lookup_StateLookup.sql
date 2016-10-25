CREATE TABLE [lookup].[StateLookup]
(
[StateCode] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[StateName] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
