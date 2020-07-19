if not exists
(
    select null
    from information_schema.tables
    where table_name = 'TicketingOrder'
)
begin
    create table [dbo].[TicketingOrder](
    	[OrderId] [int] identity(1,1) not null,
    	[MeetingId] [int] not null,
    	[Status] [varchar](25) not null,
    	[SelectedSeats] [varchar](1000) not null,
    	[Count] [int] not null,
    	[TotalPrice] [money] not null,
    	[PurchaseDate] [datetime] null,
    	[PeopleId] [int] null,
    	[CreatedDate] [datetime] not null,
     constraint [PK_TicketingOrder] primary key clustered
    (
    	[OrderId] asc
    )with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off) on [primary]
    ) on [primary]

    alter table [dbo].[TicketingOrder]  with check add  constraint [FK_TicketingOrder_Meetings] foreign key([MeetingId])
    references [dbo].[Meetings] ([MeetingId])

    alter table [dbo].[TicketingOrder] check constraint [FK_TicketingOrder_Meetings]
end
