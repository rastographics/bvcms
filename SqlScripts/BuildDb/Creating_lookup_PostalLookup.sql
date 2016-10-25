CREATE TABLE [lookup].[PostalLookup]
(
[PostalCode] [nvarchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[CityName] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StateCode] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CountryName] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ResCodeId] [int] NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
