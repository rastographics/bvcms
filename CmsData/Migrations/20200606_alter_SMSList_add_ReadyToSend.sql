if col_length('dbo.SMSList', 'ReadyToSend') is null
    alter table dbo.SMSList add ReadyToSend bit null;
