CREATE TABLE [dbo].[EnrollmentTransaction]
(
[TransactionId] [int] NOT NULL IDENTITY(1, 1),
[TransactionStatus] [bit] NOT NULL CONSTRAINT [DF_ENROLLMENT_TRANSACTION_TBL_TRANSACTION_STATUS] DEFAULT ((0)),
[CreatedBy] [int] NULL,
[CreatedDate] [datetime] NULL,
[TransactionDate] [datetime] NOT NULL,
[TransactionTypeId] [int] NOT NULL,
[OrganizationId] [int] NOT NULL,
[OrganizationName] [nvarchar] (100) NOT NULL,
[PeopleId] [int] NOT NULL,
[MemberTypeId] [int] NOT NULL,
[EnrollmentDate] [datetime] NULL,
[AttendancePercentage] [real] NULL,
[NextTranChangeDate] [datetime] NULL,
[EnrollmentTransactionId] [int] NULL,
[Pending] [bit] NULL,
[InactiveDate] [datetime] NULL,
[UserData] [nvarchar] (max) NULL,
[Request] [nvarchar] (140) NULL,
[ShirtSize] [nvarchar] (20) NULL,
[Grade] [int] NULL,
[Tickets] [int] NULL,
[RegisterEmail] [nvarchar] (80) NULL,
[TranId] [int] NULL,
[Score] [int] NOT NULL CONSTRAINT [DF_EnrollmentTransaction_Score] DEFAULT ((0)),
[SmallGroups] [nvarchar] (500) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
