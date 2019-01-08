CREATE VIEW [export].[XpPeople] AS 
SELECT PeopleId ,
       FamilyId ,
       TitleCode ,
       FirstName ,
       MiddleName ,
       MaidenName ,
       LastName ,
       SuffixCode ,
       NickName ,
       AddressLineOne ,
       AddressLineTwo ,
       CityName ,
       StateCode ,
       ZipCode ,
       CountryName ,
       CellPhone ,
       WorkPhone ,
       HomePhone ,
       EmailAddress ,
       EmailAddress2 ,
       SendEmailAddress1 ,
       SendEmailAddress2 ,
       BirthMonth ,
       BirthDay ,
       BirthYear ,
       Gender = g.Description ,
       PositionInFamily = pos.Description ,
       DoNotMailFlag ,
       DoNotCallFlag ,
       DoNotVisitFlag ,
       AddressType = atyp.Description ,
       MaritalStatus = ma.Description,
       MemberStatus = ms.Description,
       DropType = dt.Description,
       Origin = ori.Description,
       EntryPoint = ep.Description,
       InterestPoint = ip.Description,
       BaptismType = bt.Description,
       BaptismStatus = bs.Description,
       DecisionTypeId = de.Description,
       NewMemberClassStatus = nm.Description,
       LetterStatus = ls.Description,
       JoinCode = jt.Description,
       EnvelopeOption = eo.Description,
       ResCode = rc.Description,
       WeddingDate ,
       OriginDate ,
       BaptismSchedDate ,
       BaptismDate ,
       DecisionDate ,
       LetterDateRequested ,
       LetterDateReceived ,
       JoinDate ,
       DropDate ,
       DeceasedDate ,
       OtherPreviousChurch ,
       OtherNewChurch ,
       SchoolOther ,
       EmployerOther ,
       OccupationOther ,
       HobbyOther ,
       SkillOther ,
       InterestOther ,
       LetterStatusNotes ,
       Comments ,
       ContributionsStatement ,
       StatementOption = so.Description,
       SpouseId ,
       Grade ,
       BibleFellowshipClassId ,
       CampusId ,
       AltName ,
       CustodyIssue ,
       OkTransport ,
       NewMemberClassDate ,
       ReceiveSMS ,
       DoNotPublishPhones ,
       ElectronicStatement
FROM dbo.People p
left JOIN lookup.Gender g ON g.Id = p.GenderId
left JOIN lookup.FamilyPosition pos ON pos.Id = p.PositionInFamilyId
left JOIN lookup.AddressType atyp ON atyp.Id = p.AddressTypeId
LEFT JOIN lookup.MaritalStatus ma ON ma.Id = p.MaritalStatusId
LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
LEFT JOIN lookup.DropType dt ON dt.Id = p.DropCodeId
LEFT JOIN lookup.Origin ori ON ori.Id = p.OriginId
LEFT JOIN lookup.EntryPoint ep ON ep.Id = p.EntryPointId
LEFT JOIN lookup.InterestPoint ip ON ip.Id = p.InterestPointId
LEFT JOIN lookup.BaptismType bt ON bt.Id = p.BaptismTypeId
LEFT JOIN lookup.BaptismStatus bs ON bs.Id = p.BaptismStatusId
LEFT JOIN lookup.DecisionType de ON de.Id = p.DecisionTypeId
LEFT JOIN lookup.NewMemberClassStatus nm ON nm.Id = p.NewMemberClassStatusId
LEFT JOIN lookup.MemberLetterStatus ls ON ls.Id = p.LetterStatusId
LEFT JOIN lookup.JoinType jt ON jt.Id = p.JoinCodeId
LEFT JOIN lookup.EnvelopeOption eo ON eo.Id = p.EnvelopeOptionsId
LEFT JOIN lookup.EnvelopeOption so ON so.Id = p.ContributionOptionsId
LEFT JOIN lookup.ResidentCode rc ON rc.Id = p.ResCodeId
LEFT JOIN lookup.Campus ca ON ca.Id = p.CampusId

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
