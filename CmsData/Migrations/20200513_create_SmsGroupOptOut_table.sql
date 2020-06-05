if not exists
(
    select NULL
    from INFORMATION_SCHEMA.TABLES
    where TABLE_NAME = 'SmsGroupOptOut'
)
begin
	create table dbo.SmsGroupOptOut(
		ToPeopleId int not null,
		FromGroup int not null,
		Date datetime null,
	 constraint PK_SmsGroupOptOut_1 primary key clustered 
	(
		ToPeopleId asc,
		FromGroup asc
	)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
	) on [PRIMARY]

	alter table dbo.SmsGroupOptOut add  constraint DF_SmsGroupOptOut_Date  default (getdate()) for Date

	alter table dbo.SmsGroupOptOut  with check add  constraint FK_SmsGroupOptOut_People foreign key(ToPeopleId)
	references dbo.People (PeopleId)

	alter table dbo.SmsGroupOptOut check constraint FK_SmsGroupOptOut_People
end
