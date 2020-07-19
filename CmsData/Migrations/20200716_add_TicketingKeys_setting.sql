--Boolean = 1,
--Date = 2,
--Text = 3,
--Obscured = 4,
--Int = 5

--1	Administration
--2	Church Info
--3	Contributions
--4	Security
--5	Check-In
--6	Contacts
--7	Mobile App
--8	Registrations
--9	Resources
--10	Small Group Finder
--11	Protect My Ministry
--12	Pushpay
--13	Rackspace
--14	Twilio
--15	eSPACE

declare @name nvarchar(50) = 'TicketingWorkspaceKey'

if not exists(select null from dbo.Setting where Id = @name)
    insert dbo.Setting (Id, [System] ) values (@name, 1)

set @name = 'TicketingWorkspaceSecretKey'

if not exists(select null from dbo.Setting where Id = @name)
    insert dbo.Setting (Id, [System] ) values (@name, 1)

set @name = 'TicketingEnabled'
if not exists(select null from dbo.SettingMetadata where SettingId = @name)
	insert dbo.SettingMetadata ( SettingId, DataType, SettingCategoryId, Description )
	values ( @name, 1, 8,
		'Enable Ticketing: Contact support to add required keys' )

