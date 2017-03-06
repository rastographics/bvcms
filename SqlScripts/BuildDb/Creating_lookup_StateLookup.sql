CREATE TABLE [lookup].[StateLookup]
(
[StateCode] [nvarchar] (10) NOT NULL,
[StateName] [nvarchar] (30) NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
