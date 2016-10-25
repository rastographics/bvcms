CREATE TABLE [dbo].[Families]
(
[FamilyId] [int] NOT NULL IDENTITY(1, 1),
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NULL,
[RecordStatus] [bit] NOT NULL CONSTRAINT [DF_FAMILIES_TBL_RECORD_STATUS] DEFAULT ((0)),
[BadAddressFlag] [bit] NULL,
[AltBadAddressFlag] [bit] NULL,
[ResCodeId] [int] NULL,
[AltResCodeId] [int] NULL,
[AddressFromDate] [datetime] NULL,
[AddressToDate] [datetime] NULL,
[AddressLineOne] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AddressLineTwo] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CityName] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StateCode] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ZipCode] [nvarchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL CONSTRAINT [DF_Families_ZipCode] DEFAULT (''),
[CountryName] [nvarchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[StreetName] [nvarchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[HomePhone] [nvarchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[HeadOfHouseholdId] [int] NULL,
[HeadOfHouseholdSpouseId] [int] NULL,
[CoupleFlag] [int] NULL,
[HomePhoneLU] [char] (7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[HomePhoneAC] [char] (3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Comments] [nvarchar] (3000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PictureId] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
