CREATE TABLE [dbo].[OrganizationMembers]
(
[OrganizationId] [int] NOT NULL,
[PeopleId] [int] NOT NULL,
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[MemberTypeId] [int] NOT NULL,
[EnrollmentDate] [datetime] NULL,
[ModifiedBy] [int] NULL,
[ModifiedDate] [datetime] NULL,
[InactiveDate] [datetime] NULL,
[AttendStr] [nvarchar] (300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AttendPct] [real] NULL,
[LastAttended] [datetime] NULL,
[Pending] [bit] NULL,
[UserData] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Amount] [money] NULL,
[Request] [nvarchar] (140) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ShirtSize] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Grade] [int] NULL,
[Tickets] [int] NULL,
[Moved] [bit] NULL,
[RegisterEmail] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AmountPaid] [money] NULL,
[PayLink] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TranId] [int] NULL,
[Score] [int] NOT NULL CONSTRAINT [DF_OrganizationMembers_Score] DEFAULT ((0)),
[DatumId] [int] NULL,
[Hidden] [bit] NULL,
[SkipInsertTriggerProcessing] [bit] NULL,
[RegistrationDataId] [int] NULL,
[OnlineRegData] [xml] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
