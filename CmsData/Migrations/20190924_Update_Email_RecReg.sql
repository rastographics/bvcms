UPDATE r
SET r.[email] = p.[EmailAddress]
FROM [RecReg] as r
RIGHT JOIN [People] as p
ON r.[PeopleId] = p.[PeopleId]
WHERE r.[PeopleId] = p.[PeopleId]
AND r.[email] IS NULL