if not exists(select null from sys.columns 
          where Name = 'ReplyWords'
          and Object_ID = object_id('dbo.SmsNumbers'))
begin
	alter table dbo.SmsNumbers
	add ReplyWords varchar(max)
end
go

