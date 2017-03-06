CREATE TABLE [dbo].[Contact]
(
[ContactId] [int] NOT NULL IDENTITY(200000, 1),
[CreatedBy] [int] NOT NULL,
[CreatedDate] [datetime] NOT NULL,
[ContactTypeId] [int] NULL,
[ContactDate] [datetime] NOT NULL,
[ContactReasonId] [int] NULL,
[MinistryId] [int] NULL,
[NotAtHome] [bit] NULL,
[LeftDoorHanger] [bit] NULL,
[LeftMessage] [bit] NULL,
[GospelShared] [bit] NULL,
[PrayerRequest] [bit] NULL,
[ContactMade] [bit] NULL,
[GiftBagGiven] [bit] NULL,
[Comments] [nvarchar] (max) NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[LimitToRole] [nvarchar] (50) NULL,
[OrganizationId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
