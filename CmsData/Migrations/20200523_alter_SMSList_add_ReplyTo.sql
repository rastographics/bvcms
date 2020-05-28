IF COL_LENGTH('dbo.SMSList', 'ReplyToId') IS NULL
    alter table dbo.SMSList add ReplyToId int null;
