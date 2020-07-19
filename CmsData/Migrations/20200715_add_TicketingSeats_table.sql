if not exists
(
    select null
    from information_schema.tables
    where table_name = 'TicketingSeats'
)
begin
    create table [dbo].[TicketingSeats](
        [OrderId] [int] not null,
        [SeatLabel] [varchar](25) not null,
        [Price] [money] null,
        [Category] [varchar](25) null,
        [CategoryKey] [int] null,
        [Section] [varchar](25) null,
        [Row] [varchar](10) null,
        [Seat] [int] null,
    constraint [PK_TicketingSeats] primary key clustered
    (
        [OrderId] asc,
        [SeatLabel] asc
    )with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on, optimize_for_sequential_key = off) on [primary]
    ) on [PRIMARY]

    alter table [dbo].[TicketingSeats]  with check add  constraint [FK_TicketingSeats_TicketingOrder] foreign key([OrderId])
    references [dbo].[TicketingOrder] ([OrderId])

    alter table [dbo].[TicketingSeats] check constraint [FK_TicketingSeats_TicketingOrder]
end
