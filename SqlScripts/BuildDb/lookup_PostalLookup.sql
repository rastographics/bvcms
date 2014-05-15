CREATE TABLE [lookup].[PostalLookup]
(
[PostalCode] [nvarchar] (15) NOT NULL,
[CityName] [nvarchar] (20) NULL,
[StateCode] [nvarchar] (20) NULL,
[CountryName] [nvarchar] (30) NULL,
[ResCodeId] [int] NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
