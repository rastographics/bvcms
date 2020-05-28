if not exists(select null from sys.columns 
          where Name = 'ReplyWords'
          and Object_ID = object_id('dbo.SmsGroups'))
begin
	alter table dbo.SmsGroups
	add ReplyWords varchar(max)
end
go

