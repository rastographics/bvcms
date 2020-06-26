if col_length('dbo.SMSList', 'Sent') is null
    alter table dbo.SMSList add Sent bit null;
