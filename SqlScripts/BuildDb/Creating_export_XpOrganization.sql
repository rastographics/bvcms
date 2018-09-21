CREATE VIEW [export].[XpOrganization] AS 
SELECT OrganizationId ,
       OrganizationName ,
       [Location] ,
       OrganizationStatus = os.[Description],
       DivisionId ,
       LeaderMemberType = le.[Description] ,
       GradeAgeStart ,
       GradeAgeEnd ,
       FirstMeetingDate ,
       LastMeetingDate ,
       ParentOrgId ,
       LeaderId ,
       Gender = ge.[Description] ,
       o.[Description] ,
       BirthDayStart ,
       BirthDayEnd ,
       PhoneNumber ,
       IsBibleFellowshipOrg ,
       Offsite ,
       OrganizationType = ot.[Description],
       PublishDirectory ,
       IsRecreationTeam ,
       NotWeekly ,
       IsMissionTrip 
FROM dbo.Organizations o
LEFT JOIN lookup.MemberType le ON le.Id = LeaderMemberTypeId
LEFT JOIN lookup.Gender ge ON ge.Id = GenderId
LEFT JOIN lookup.OrganizationType ot ON ot.Id = o.OrganizationTypeId
LEFT JOIN lookup.OrganizationStatus os ON os.Id = o.OrganizationStatusId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
