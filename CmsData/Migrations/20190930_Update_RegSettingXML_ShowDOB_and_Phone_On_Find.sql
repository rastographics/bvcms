UPDATE [Organizations]
SET [RegSettingXml].modify('insert <ShowDOBOnFind>True</ShowDOBOnFind> into (/Settings/NotRequired)[1]')
FROM [dbo].[Organizations] AS [a]
WHERE [a].[RegSettingXml] IS NOT NULL
AND CAST(IIF(RegSettingXml.value('(//ShowDOBOnFind)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT) IS NULL
GO

UPDATE [Organizations]
SET [RegSettingXml].modify('insert <ShowPhoneOnFind>True</ShowPhoneOnFind> into (/Settings/NotRequired)[1]')
FROM [dbo].[Organizations] AS [a]
WHERE [a].[RegSettingXml] IS NOT NULL
AND CAST(IIF(RegSettingXml.value('(//ShowPhoneOnFind)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT) IS NULL
GO