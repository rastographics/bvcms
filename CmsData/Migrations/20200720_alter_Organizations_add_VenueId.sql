if col_length('dbo.Organizations', 'VenueId') is null
    alter table dbo.Organizations add VenueId varchar(50) null;
