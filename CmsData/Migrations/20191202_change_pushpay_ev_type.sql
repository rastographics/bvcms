update PeopleExtra set [Data] = StrValue where Field like '%PushPayKey%' and [Data] is null
go
update PeopleExtra set StrValue = null where Field like '%PushPayKey%' and StrValue is not null
go
