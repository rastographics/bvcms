CREATE TABLE [lookup].[PostalLookup]
(
[PostalCode] [nvarchar] (15) NOT NULL,
[CityName] [nvarchar] (20) NULL,
[StateCode] [nvarchar] (20) NULL,
[CountryName] [nvarchar] (30) NULL,
[ResCodeId] [int] NULL,
[Hardwired] [bit] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
