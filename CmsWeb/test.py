orgid = 2191107

sql = '''
WITH members AS (
	SELECT om.PeopleId, 
		LastGroup = (SELECT MAX(Value) FROM dbo.Split(Groups, CHAR(10)) WHERE Value LIKE 'NewGuestEmail-%')
	FROM dbo.OrgPeopleCurrent({0}) om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	JOIN dbo.OrganizationMembers omm ON omm.PeopleId = p.PeopleId AND omm.OrganizationId = {0}
	WHERE omm.MemberTypeId = 220
),
sends AS (
	SELECT om.PeopleId, 
		SendNumber = CONVERT(INT, RIGHT(ISNULL(om.LastGroup, '0'), 1)) + 1
	FROM members om
)
SELECT PeopleId FROM sends WHERE SendNumber = {1}
'''

# build queries for each email to send
qlist = []
for n in range(6):
    nn = str(n+1)
    query = q.SqlPeopleIdsToQuery(sql.format(orgid, nn))
    qlist.append(query)
    print nn, query, '<br>'
    
# use each query to send email and update sub-group
for n in range(6):
    nn = str(n+1)
    query = qlist[n]
    model.EmailContent(query, 2920018, 'karen@bvcms.com', 'Karen Worrell', 'NewGuestEmail-' + nn)
    # model.AddSubGroupFromQuery(query, orgid, 'NewGuestEmail-' + nn)
