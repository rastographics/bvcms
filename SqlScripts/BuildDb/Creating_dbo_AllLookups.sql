CREATE VIEW [dbo].[AllLookups] AS 
 
SELECT Name = 'AddressType', Id, Description FROM lookup.AddressType 
UNION 
SELECT Name = 'AttendCredit', Id, Description FROM lookup.AttendCredit 
UNION 
SELECT Name = 'AttendType', Id, Description FROM lookup.AttendType 
UNION 
SELECT Name = 'BaptismStatus', Id, Description FROM lookup.BaptismStatus 
UNION 
SELECT Name = 'BaptismType', Id, Description FROM lookup.BaptismType 
UNION 
SELECT Name = 'BundleHeaderTypes', Id, Description FROM lookup.BundleHeaderTypes 
UNION 
SELECT Name = 'BundleStatusTypes', Id, Description FROM lookup.BundleStatusTypes 
UNION 
SELECT Name = 'Campus', Id, Description FROM lookup.Campus 
UNION 
SELECT Name = 'ContactReason', Id, Description FROM lookup.ContactReason 
UNION 
SELECT Name = 'ContactType', Id, Description FROM lookup.ContactType 
UNION 
SELECT Name = 'ContributionStatus', Id, Description FROM lookup.ContributionStatus 
UNION 
SELECT Name = 'ContributionType', Id, Description FROM lookup.ContributionType 
UNION 
SELECT Name = 'DecisionType', Id, Description FROM lookup.DecisionType 
UNION 
SELECT Name = 'DropType', Id, Description FROM lookup.DropType 
UNION 
SELECT Name = 'EntryPoint', Id, Description FROM lookup.EntryPoint 
UNION 
SELECT Name = 'EnvelopeOption', Id, Description FROM lookup.EnvelopeOption 
UNION 
SELECT Name = 'FamilyMemberType', Id, Description FROM lookup.FamilyMemberType 
UNION 
SELECT Name = 'FamilyPosition', Id, Description FROM lookup.FamilyPosition 
UNION 
SELECT Name = 'FamilyRelationship', Id, Description FROM lookup.FamilyRelationship 
UNION 
SELECT Name = 'Gender', Id, Description FROM lookup.Gender 
UNION 
SELECT Name = 'JoinType', Id, Description FROM lookup.JoinType 
UNION 
SELECT Name = 'MaritalStatus', Id, Description FROM lookup.MaritalStatus 
UNION 
SELECT Name = 'MeetingType', Id, Description FROM lookup.MeetingType 
UNION 
SELECT Name = 'MemberLetterStatus', Id, Description FROM lookup.MemberLetterStatus 
UNION 
SELECT Name = 'MemberStatus', Id, Description FROM lookup.MemberStatus 
UNION 
SELECT Name = 'MemberType', Id, Description FROM lookup.MemberType 
UNION 
SELECT Name = 'NewMemberClassStatus', Id, Description FROM lookup.NewMemberClassStatus 
UNION 
SELECT Name = 'OrganizationStatus', Id, Description FROM lookup.OrganizationStatus 
UNION 
SELECT Name = 'OrganizationType', Id, Description FROM lookup.OrganizationType 
UNION 
SELECT Name = 'Origin', Id, Description FROM lookup.Origin 
UNION 
SELECT Name = 'ResidentCode', Id, Description FROM lookup.ResidentCode 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
