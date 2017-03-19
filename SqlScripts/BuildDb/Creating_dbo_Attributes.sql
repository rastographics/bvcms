
CREATE VIEW [dbo].[Attributes] AS 
	SELECT PeopleId, Field, name, value, FieldAttr = Field + ':' + name + ':' + value
	FROM dbo.PeopleExtra
	CROSS APPLY OPENJSON(Data) WITH([name] VARCHAR(200), [value] VARCHAR(MAX))
	WHERE Type = 'Attr'

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
