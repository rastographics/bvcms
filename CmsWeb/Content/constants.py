class OriginCode(object):
    Visit = 10
    Request = 40
    PhoneIn = 50
    SurveyEE = 60
    Enrollment = 70
    Contribution = 90
    NewFamilyMember = 100

class DecisionCode(object):
    Unknown = 0
    ProfessionForMembership = 10
    ProfessionNotForMembership = 20
    Letter = 30
    Statement = 40
    StatementReqBaptism = 50
    Cancelled = 60

class MemberStatusCode(object):
     Member = 10
     NotMember = 20
     Pending = 30
     Previous = 40
     JustAdded = 50

class NewMemberClassStatusCode(object):
     NotSpecified = 0
     Pending = 10
     Attended = 20
     AdminApproval = 30
     GrandFathered = 40
     ExemptedChild = 50
     Unknown = 9

class BaptismTypeCode(object):
     NotSpecified = 0
     Original = 10
     Subsequent = 20
     Biological = 30
     NonMember = 40
     Required = 50

class BaptismStatusCode(object):
     NotSpecified = 0
     Scheduled = 10
     NotScheduled = 20
     Completed = 30
     Canceled = 40

class JoinTypeCode(object):
     Unknown = 0
     BaptismPOF = 10
     BaptismSRB = 20
     BaptismBIO = 30
     Statement = 40
     Letter = 50

class DropTypeCode(object):
     Administrative = 20
     AnotherDenomination = 60
     LetteredOut = 40
     Requested = 50
     Other = 98
     NotDropped = 0
     Duplicate = 10
     Deceased = 30

class EnvelopeOptionCode(object):
     NoEnvelope = 0
     Individual = 1
     Joint = 2

class MaritalStatusCode(object):
    Unknown = 0
    Single = 10
    Married = 20
    Separated = 30
    Divorced = 40
    Widowed = 50

class PositionInFamily(object):
    PrimaryAdult = 10
    SecondaryAdult = 20
    Child = 30

class MemberTypeCode(object):
    Administrator = 100
    President = 101
    Leader = 140
    AssistantLeader = 142
    Teacher = 160
    AssistantTeacher = 161
    Member = 220
    InActive = 230
    VisitingMember = 300
    Visitor = 310
    InServiceMember = 500
    VIP = 700
    Drop = -1

class OrgStatusCode(object):
    Active = 30
    Inactive = 40

class AttendTrackLevelCode(object):
    NoTracking = 0
    Headcount = 10
    Individual = 20
    Registered = 30

class AttendanceClassificationCode(object):
    Normal = 0

class RegistrationEnum(object):
    NoRegistration = 0
    JoinOrganization = 1
    AttendMeeting = 2
    UserSelectsOrganization = 3
    ComputeOrganizationByAge = 4
    CreateAccount = 5
    ChooseSlot = 6
    ManageSubscriptions = 7

class AttendTypeCode(object):
    Absent = 0
    Leader = 10
    Volunteer = 20
    Member = 30
    VisitingMember = 40
    RecentVisitor = 50
    NewVisitor = 60
    InService = 70
    Offsite = 80
    Group = 90
    Homebound = 100
    OtherClass = 110

class TaskStatusCode(object):
    Active = 10
    Waiting = 20
    Someday = 30
    Complete = 40
    Pending = 50
    Redelegated = 60

class ContactTypeCode(object):
    PersonalVisit = 1
    PhoneCall = 2
    LetterSent = 3
    CardSent = 4
    EmailSent = 5
    InfoPackSent = 6
    Other = 7
    PhoneIn = 11
    SurveyEE = 12

class ContactReasonCode(object):
    Unknown = 99
    Bereavement = 100
    Health = 110
    Personal = 120
    OutReach = 130
    ComeAndSee = 131
    InReach = 140
    Information = 150
    Other = 160