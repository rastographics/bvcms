CREATE TABLE [dbo].[PaymentInfo]
(
[PeopleId] [int] NOT NULL,
[AuNetCustId] [int] NULL,
[AuNetCustPayId] [int] NULL,
[SageBankGuid] [uniqueidentifier] NULL,
[SageCardGuid] [uniqueidentifier] NULL,
[MaskedAccount] [nvarchar] (30) NULL,
[MaskedCard] [nvarchar] (30) NULL,
[Expires] [nvarchar] (10) NULL,
[testing] [bit] NULL,
[PreferredGivingType] [nvarchar] (2) NULL,
[PreferredPaymentType] [nvarchar] (2) NULL,
[Routing] [nvarchar] (10) NULL,
[FirstName] [nvarchar] (50) NULL,
[MiddleInitial] [nvarchar] (10) NULL,
[LastName] [nvarchar] (50) NULL,
[Suffix] [nvarchar] (10) NULL,
[Address] [nvarchar] (50) NULL,
[City] [nvarchar] (50) NULL,
[State] [nvarchar] (10) NULL,
[Zip] [nvarchar] (15) NULL,
[Phone] [nvarchar] (25) NULL,
[TbnBankVaultId] [int] NULL,
[TbnCardVaultId] [int] NULL,
[AuNetCustPayBankId] [int] NULL,
[BluePayCardVaultId] [nvarchar] (50) NULL,
[Address2] [nvarchar] (50) NULL,
[Country] [nvarchar] (50) NULL
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
