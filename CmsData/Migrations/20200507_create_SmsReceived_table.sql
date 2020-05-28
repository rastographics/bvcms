if not exists
(
    select NULL
    from INFORMATION_SCHEMA.TABLES
    where TABLE_NAME = 'SmsReceived'
)
begin
    create table dbo.SmsReceived
    (
        Id int identity(1,1) not null,
        DateReceived datetime null,
        FromNumber varchar(15) null,
        FromPeopleId int null,
        ToNumber varchar(15) null,
        ToGroupId int null,
        Body varchar(max) null,
        Action varchar(25) null,
        Args varchar(50) null,
        ActionResponse varchar(200) null,
        ErrorOccurred bit null,
        RepliedTo bit null,
        constraint PK_SmsReceived
            primary key clustered (Id asc)
            with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on,
                  allow_page_locks = on
                 ) on [PRIMARY]
    ) on [PRIMARY] textimage_on [PRIMARY];

    alter table dbo.SmsReceived with check
    add constraint FK_SmsReceived_People
        foreign key (FromPeopleId)
        references dbo.People (PeopleId);

    alter table dbo.SmsReceived check constraint FK_SmsReceived_People;

    alter table dbo.SmsReceived with check
    add constraint FK_SmsReceived_SMSGroups
        foreign key (ToGroupId)
        references dbo.SMSGroups (ID);

    alter table dbo.SmsReceived check constraint FK_SmsReceived_SMSGroups;
end;
