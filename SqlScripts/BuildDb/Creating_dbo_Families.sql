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
[AddressLineOne] [nvarchar] (100) NULL,
[AddressLineTwo] [nvarchar] (100) NULL,
[CityName] [nvarchar] (30) NULL,
[StateCode] [nvarchar] (30) NULL,
[ZipCode] [nvarchar] (15) NULL CONSTRAINT [DF_Families_ZipCode] DEFAULT (''),
[CountryName] [nvarchar] (40) NULL,
[StreetName] [nvarchar] (40) NULL,
[HomePhone] [nvarchar] (20) NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[HeadOfHouseholdId] [int] NULL,
[HeadOfHouseholdSpouseId] [int] NULL,
[CoupleFlag] [int] NULL,
[HomePhoneLU] [char] (7) NULL,
[HomePhoneAC] [char] (3) NULL,
[Comments] [nvarchar] (3000) NULL,
[PictureId] [int] NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
