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
