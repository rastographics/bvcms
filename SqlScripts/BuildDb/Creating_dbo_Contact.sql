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
[Comments] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[LimitToRole] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[OrganizationId] [int] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
