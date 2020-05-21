IF OBJECT_ID(N'dbo.PaymentMethod', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[PaymentMethod](
	[PaymentMethodId] uniqueidentifier NOT NULL CONSTRAINT DF_PaymentMethod_PaymentMethodId DEFAULT (NEWID()),
	[PeopleId] int NOT NULL,
	[PaymentMethodTypeId] int NOT NULL,
	[IsDefault] bit NULL,
	[Name] nvarchar(max) NOT NULL,
	[BankName] nvarchar(max) NOT NULL,
	[VaultId] nvarchar(max) NOT NULL,
	[GatewayAccountId] int NOT NULL,
	[NameOnAccount] nvarchar(max) NULL,
	[MaskedDisplay] nvarchar(max) NULL,
	[Last4] nvarchar(max) NULL,
	[ExpiresMonth] int NULL,
	[ExpiresYear] int NULL
CONSTRAINT [PK_PaymentMethod_PaymentMethodId] PRIMARY KEY NONCLUSTERED ([PaymentMethodId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO

IF OBJECT_ID(N'FK_PaymentMethod_GatewayAccount') IS NULL
ALTER TABLE [dbo].[PaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethod_GatewayAccount] FOREIGN KEY([GatewayAccountId])
REFERENCES [dbo].[GatewayAccount] ([GatewayAccountId])
GO

ALTER TABLE [dbo].[PaymentMethod] CHECK CONSTRAINT [FK_PaymentMethod_GatewayAccount]
GO

IF OBJECT_ID(N'FK_PaymentMethod_People') IS NULL
ALTER TABLE [dbo].[PaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethod_People] FOREIGN KEY([PeopleId])
REFERENCES [dbo].[People] ([PeopleId])
GO

ALTER TABLE [dbo].[PaymentMethod] CHECK CONSTRAINT [FK_PaymentMethod_People]
GO

IF OBJECT_ID(N'dbo.ScheduledGift', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ScheduledGift](
	[ScheduledGiftId] uniqueidentifier NOT NULL CONSTRAINT DF_ScheduledGift_ScheduledGiftId DEFAULT (NEWID()),
	[PeopleId] int NOT NULL,
	[ScheduledGiftTypeId] int NOT NULL,
	[PaymentMethodId] uniqueidentifier NOT NULL,
	[StartDate] datetime NOT NULL,
	[EndDate] datetime NULL,
	[LastProcessed] datetime NULL
CONSTRAINT [PK_ScheduledGift_ScheduledGiftId] PRIMARY KEY NONCLUSTERED ([ScheduledGiftId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO

IF OBJECT_ID(N'FK_ScheduledGift_PaymentMethod') IS NULL
ALTER TABLE [dbo].[ScheduledGift]  WITH CHECK ADD  CONSTRAINT [FK_ScheduledGift_PaymentMethod] FOREIGN KEY([PaymentMethodId])
REFERENCES [dbo].[PaymentMethod] ([PaymentMethodId])
GO

ALTER TABLE [dbo].[ScheduledGift] CHECK CONSTRAINT [FK_ScheduledGift_PaymentMethod]
GO

IF OBJECT_ID(N'FK_ScheduledGift_People') IS NULL
ALTER TABLE [dbo].[ScheduledGift]  WITH CHECK ADD  CONSTRAINT [FK_ScheduledGift_People] FOREIGN KEY([PeopleId])
REFERENCES [dbo].[People] ([PeopleId])
GO

ALTER TABLE [dbo].[ScheduledGift] CHECK CONSTRAINT [FK_ScheduledGift_People]
GO

IF OBJECT_ID(N'dbo.ScheduledGiftAmount', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[ScheduledGiftAmount](
	[ScheduledGiftAmountId] int IDENTITY(1,1) NOT NULL,
	[ScheduledGiftId] uniqueidentifier NOT NULL,
	[FundId] int NOT NULL,
	[Amount] decimal NOT NULL
CONSTRAINT [PK_ScheduledGiftAmount_ScheduledGiftAmountId] PRIMARY KEY CLUSTERED ([ScheduledGiftAmountId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO

ALTER TABLE [dbo].[ScheduledGiftAmount]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledGiftAmounts_ScheduledGift] FOREIGN KEY([ScheduledGiftId])
REFERENCES [dbo].[ScheduledGift] ([ScheduledGiftId])
GO

ALTER TABLE [dbo].[ScheduledGiftAmount] CHECK CONSTRAINT [FK_ScheduledGiftAmounts_ScheduledGift]
GO

ALTER TABLE [dbo].[ScheduledGiftAmount]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledGiftAmounts_Fund] FOREIGN KEY([FundId])
REFERENCES [dbo].[ContributionFund] ([FundId])
GO

ALTER TABLE [dbo].[ScheduledGiftAmount] CHECK CONSTRAINT [FK_ScheduledGiftAmounts_Fund]
GO