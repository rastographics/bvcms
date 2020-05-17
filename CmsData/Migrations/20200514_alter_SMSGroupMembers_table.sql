if not exists
(
    select 1
    from sys.columns
    where name = N'ReceiveNotifications'
          and object_id = object_id(N'dbo.SMSGroupMembers')
)
begin
    alter table dbo.SMSGroupMembers add ReceiveNotifications bit null;
end;
