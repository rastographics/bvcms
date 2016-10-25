CREATE TABLE [dbo].[EnrollmentTransaction]
(
[TransactionId] [int] NOT NULL IDENTITY(1, 1),
[TransactionStatus] [bit] NOT NULL CONSTRAINT [DF_ENROLLMENT_TRANSACTION_TBL_TRANSACTION_STATUS] DEFAULT ((0)),
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[TransactionDate] [datetime] NOT NULL,
[TransactionTypeId] [int] NOT NULL,
[OrganizationId] [int] NOT NULL,
[OrganizationName] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[PeopleId] [int] NOT NULL,
[MemberTypeId] [int] NOT NULL,
[EnrollmentDate] [datetime] NULL,
[AttendancePercentage] [real] NULL,
[NextTranChangeDate] [datetime] NULL,
[EnrollmentTransactionId] [int] NULL,
[Pending] [bit] NULL,
[InactiveDate] [datetime] NULL,
[UserData] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Request] [nvarchar] (140) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ShirtSize] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Grade] [int] NULL,
[Tickets] [int] NULL,
[RegisterEmail] [nvarchar] (80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TranId] [int] NULL,
[Score] [int] NOT NULL CONSTRAINT [DF_EnrollmentTransaction_Score] DEFAULT ((0)),
[SmallGroups] [nvarchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SkipInsertTriggerProcessing] [bit] NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
