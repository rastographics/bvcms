if (select columnproperty(object_id('dbo.SMSItems', 'U'), 'PeopleID', 'AllowsNull')) = 0
begin
	alter table dbo.SMSItems alter column PeopleID int null
end