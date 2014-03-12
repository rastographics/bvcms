/*
	Migration script for ChangeDetails (logging)
	This script will convert the old logging where all changes were saved in an HTML table inside a Data column of the ChangeLog record.
	It parses the table and inserts individual entries into the ChangeDetails table.

	This script should be run only once if you have a database from or prior to the schema from the following revision.
		Revision: 0c08701ff623ebe0308743f0b36c00c024a8046d
		Author: David Carroll <david@bvcms.com>
		Date: 1/22/2014 9:04:16 PM
		Message:
		DbSchema (stored procedure)
		----
		Modified: SqlScripts/BuildStarterDatabase.sql
		Modified: SqlScripts/BuildTestDatabase.sql

	The new logging scheme is implemented in the following revision
		Revision: 5ac5dfe7ad118c00b16b5d4c4642bb42e9fafdd1
		Author: David Carroll <david@bvcms.com>
		Date: 3/10/2014 1:10:37 PM
		Message:
		Change the way change logging works, requires a data migration

*/
INSERT dbo.ChangeDetails
        ( Id, Field, Before, After )
SELECT Id
	,c.value('data(td)[1]', 'varchar(max)') [field]
	,c.value('data(td)[2]', 'varchar(max)') [before]
	,c.value('data(td)[3]', 'varchar(max)') [after]
FROM 
(
	SELECT
		Id,
		CAST(REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE(
			REPLACE( Data, 
		'<table>', '{table}'), 
		'<tr>', '{tr}'),
		'<td>','{td}'),
		'</table>', '{/table}'), 
		'</tr>', '{/tr}'),
		'</td>','{/td}'),
		'&','&amp;'),
		'<','&lt;'),
		'>','&gt;'),
		'{','<'),
		'}','>')
		AS XML) tds

FROM dbo.ChangeLog
WHERE id > 140000
) t
CROSS APPLY tds.nodes('table/tr') x(c)
