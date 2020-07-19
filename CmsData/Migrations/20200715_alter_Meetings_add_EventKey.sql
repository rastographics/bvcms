if col_length('dbo.Meetings', 'EventKey') is null
    alter table dbo.Meetings add EventKey varchar(50) null;
