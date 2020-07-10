declare @name nvarchar(50) = 'DefaultReplyUnknownNumbers'
if not exists(select null from dbo.SettingMetadata where SettingId = @name)
	insert dbo.SettingMetadata ( SettingId, DataType, SettingCategoryId, Description )
	values ( @name, 3, 14, 
		'The reply that is sent to incoming texts when a phone number is not recognized' )