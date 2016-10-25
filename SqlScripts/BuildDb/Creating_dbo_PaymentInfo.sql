CREATE TABLE [dbo].[PaymentInfo]
(
[PeopleId] [int] NOT NULL,
[AuNetCustId] [int] NULL,
[AuNetCustPayId] [int] NULL,
[SageBankGuid] [uniqueidentifier] NULL,
[SageCardGuid] [uniqueidentifier] NULL,
[MaskedAccount] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MaskedCard] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Expires] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[testing] [bit] NULL,
[PreferredGivingType] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PreferredPaymentType] [nvarchar] (2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Routing] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FirstName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MiddleInitial] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LastName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Suffix] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Address] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[City] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[State] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Zip] [nvarchar] (15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Phone] [nvarchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TbnBankVaultId] [int] NULL,
[TbnCardVaultId] [int] NULL,
[AuNetCustPayBankId] [int] NULL,
[BluePayCardVaultId] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Address2] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[Country] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
