GO
DECLARE @supportsToFix TABLE
(
ID INT IDENTITY(1,1),
orgDatum INT,
supporterDatum INT,
goerDatum INT,
amountDatum MONEY,
idGSA INT,
orgGSA INT,
supporterGSA INT,
goerGSA INT,
amountGSA MONEY
);

WITH table_xml(DatumId, xml, stampDate, orgId) AS
(
	SELECT DT.Id, DT.Data, DT.Stamp, O.OrganizationId
	FROM dbo.RegistrationData DT
	JOIN dbo.Organizations O ON O.OrganizationId = DT.OrganizationId
	WHERE DT.completed = 1 AND O.IsMissionTrip = 1
)
,supportsTransactions(OrgId, SupporterId, GoerId, Amount, Created) AS
(
	SELECT  
	xt.orgId AS OrgId,
	x.XmlCol.value('(UserPeopleId)[1]','INT') AS UserPeopleId,
	x.XmlCol.value('(List/OnlineRegPersonModel/MissionTripGoerId)[1]','INT') AS GoerId,
	x.XmlCol.value('(List/OnlineRegPersonModel/MissionTripSupportGoer)[1]','MONEY') AS GoerSuppport,
	CAST(xt.stampDate AS DATE) AS StampDate
	FROM table_xml xt
	CROSS APPLY xt.xml.nodes('/OnlineRegModel') x(XmlCol)
	WHERE 
	x.XmlCol.value('(List/OnlineRegPersonModel/MissionTripGoerId)[1]','INT') IS NOT NULL
	AND
	x.XmlCol.value('(UserPeopleId)[1]','INT') IS NOT NULL
	AND
	x.XmlCol.value('(UserPeopleId)[1]','INT') <> x.XmlCol.value('(List/OnlineRegPersonModel/MissionTripGoerId)[1]','INT')
	AND
	x.XmlCol.value('(List/OnlineRegPersonModel/MissionTripSupportGoer)[1]','MONEY') IS NOT NULL
)
,compareTable(orgDatum, supporterDatum, goerDatum, amountDatum, idGSA, orgGSA, supporterGSA, goerGSA, amountGSA) AS
(
	SELECT 
	st.OrgId,
	st.SupporterId,
	st.GoerID, 
	st.Amount,
	gsa.Id,
	gsa.OrgId,
	gsa.SupporterId,
	gsa.GoerId, 
	gsa.Amount 
	FROM supportsTransactions st
	JOIN dbo.GoerSenderAmounts gsa ON 
	gsa.OrgId = st.OrgId AND
	gsa.GoerId = st.GoerID AND
	gsa.Amount = st.Amount AND
	CAST(gsa.Created AS DATE) = st.Created
)
INSERT INTO @supportsToFix
(
    orgDatum,
    supporterDatum,
    goerDatum,
    amountDatum,
	idGSA,
    orgGSA,
    supporterGSA,
    goerGSA,
    amountGSA
)
SELECT ct.*
FROM compareTable ct
WHERE ct.supporterDatum <> ct.supporterGSA
ORDER BY goerGSA;

UPDATE A
SET A.SupporterId = B.supporterDatum
FROM dbo.GoerSenderAmounts AS A
INNER JOIN @supportsToFix AS B ON A.Id = B.idGSA
WHERE A.Id = B.idGSA

GO
