IF (select count(*) from SettingMetadata where [SettingId] like 'CreditBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('CreditBase', NULL, 'This is the base fee you want to recoup for credit cards when the donor covers the fee. It should be a dollar amount (example: .30).', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'CreditPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('CreditPercent', NULL, 'This is the percent fee you want to recoup for credit card when the donor covers the fee. It should be a number (example: 1.9).', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'DebitBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('DebitBase', NULL, 'This is the base fee you want to recoup for debit cards when the donor covers the fee. It should be a dollar amount (example: .30).', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'DebitPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('DebitPercent', NULL, 'This is the percent fee you want to recoup for debit cards when the donor covers the fee. It should be a number (example: 1.5).', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'ACHBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('ACHBase', NULL, 'This is the base fee you want to recoup for ACH when the donor covers the fee. It should be a dollar amount (example: .30).', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'ACHPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('ACHPercent', NULL, 'This is the percent fee you want to recoup for ACH when the donor covers the fee. It should be a number (example: .5).', 5, 3, NULL)
END
GO
