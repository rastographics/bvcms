if not exists (select * from sys.tables t JOIN sys.schemas s on (t.schema_id = s.schema_id)
where s.name = 'custom' and t.name = 'JsonDocumentRecords') 
    create table [custom].[JsonDocumentRecords](
    	[Section] [nvarchar](50) not null,
    	[Id1] [nvarchar](50) not null,
    	[Id2] [nvarchar](50) not null,
    	[Id3] [nvarchar](50) not null,
    	[Id4] [nvarchar](50) not null,
    	[Json] [nvarchar](max) null,
     constraint [PK_JsonDocumentRecords] primary key clustered 
    (
    	[Section] asc,
    	[Id1] asc,
    	[Id2] asc,
    	[Id3] asc,
    	[Id4] asc
    )with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
    ) on [PRIMARY] textimage_on [PRIMARY]
go
drop procedure if exists [custom].[AddUpdateJsonRecord]
go
create procedure [custom].AddUpdateJsonRecord
	@json nvarchar(max), 
	@section nvarchar(50), 
	@id1 nvarchar(50), 
	@id2 nvarchar(50), 
	@id3 nvarchar(50), 
	@id4 nvarchar(50)
as 
begin 
	if exists(select null from [custom].JsonDocumentRecords where Section = @section and Id1 = @id1 and Id2 = @id2 and Id3 = @id3 and Id4 = @id4) 
		update custom.JsonDocumentRecords
		set Json = @json
		where Section = @section and Id1 = @id1 and Id2 = @id2 and Id3 = @id3 and Id4 = @id4
	else
		insert [custom].JsonDocumentRecords (Section, Id1, Id2, Id3, Id4, [Json])
		values (@section, @id1, @id2, @id3, @id4, @json)
end
go
drop procedure if exists [custom].[DeleteJsonRecord]
go
create procedure [custom].DeleteJsonRecord
	@section nvarchar(50), 
	@id1 nvarchar(50), 
	@id2 nvarchar(50), 
	@id3 nvarchar(50), 
	@id4 nvarchar(50)
as 
begin 
	delete [custom].JsonDocumentRecords
	where Section = @section and Id1 = @id1 and Id2 = @id2 and Id3 = @id3 and Id4 = @id4
end
go
