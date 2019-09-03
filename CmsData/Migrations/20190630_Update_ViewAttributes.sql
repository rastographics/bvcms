alter view [dbo].[Attributes] as 
	select PeopleId, Field, [key] name, value, Field collate database_default + ':' + [Key] + ':' + Value FieldAttr
	from dbo.PeopleExtra e
	outer apply openjson(Data) 
	where e.Type = 'Attr'
