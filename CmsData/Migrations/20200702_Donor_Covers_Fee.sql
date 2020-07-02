IF (select count(*) from SettingMetadata where [SettingId] like 'CreditBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('CreditBase', NULL, '', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'CreditPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('CreditPercent', NULL, '', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'DebitBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('DebitBase', NULL, '', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'DebitPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('DebitPercent', NULL, '', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'ACHBase') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('ACHBase', NULL, '', 5, 3, NULL)
END
GO

IF (select count(*) from SettingMetadata where [SettingId] like 'ACHPercent') = 0
BEGIN
INSERT INTO [dbo].SettingMetadata
           ([SettingId], [DisplayName], [Description], [DataType], [SettingCategoryId], [DefaultValue])
     VALUES
           ('ACHPercent', NULL, '', 5, 3, NULL)
END
GO
