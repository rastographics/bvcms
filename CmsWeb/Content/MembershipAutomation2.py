 
# this is an IronPython script for MembershipAutomation in BVCMS
# the variable p has been passed in and is the person that we are saving Member Profile information for

#import useful constants (defined in constants.py)
from constants import *


# define all functions first, codes starts running below functions

# do not allow empty join date
def SetJoinDate(p, name, dt):
    if dt == None:
        p.errorReturn = "need a " + name + " date"
    p.JoinDate = dt

# this controls the membership status, makes them a member if they have completed the two requirements
def CheckJoinStatus(p):

    if (p.DecisionTypeId == DecisionCode.ProfessionForMembership):
        if (p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId) 
		and p.BaptismStatusId == BaptismStatusCode.Completed):
            if (p.NewMemberClassDate != None and p.BaptismDate != None):
                if p.NewMemberClassDate > p.BaptismDate:
                    SetJoinDate(p, "NewMemberClass", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Baptism", p.BaptismDate)
            p.MemberStatusId = MemberStatusCode.Member
            if p.BaptismTypeId == BaptismTypeCode.Biological:
                p.JoinCodeId = JoinTypeCode.BaptismBIO
            else:
                p.JoinCodeId = JoinTypeCode.BaptismPOF

    elif (p.DecisionTypeId == DecisionCode.Letter):
        if (p.NewMemberClassStatusIdChanged):
            if (p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId)
			or p.NewMemberClassStatusId == NewMemberClassStatusCode.AdminApproval):
                p.MemberStatusId = MemberStatusCode.Member
                p.JoinCodeId = JoinTypeCode.Letter
                if (p.NewMemberClassDate != None):
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Decision", p.DecisionDate)

    elif (p.DecisionTypeId == DecisionCode.Statement):
        if (p.NewMemberClassStatusIdChanged):
            if (p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId)):
                p.MemberStatusId = MemberStatusCode.Member
                p.JoinCodeId = JoinTypeCode.Statement
                if (p.NewMemberClassDate != None):
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                else:
                    SetJoinDate(p, "Decision", p.DecisionDate)

    elif (p.DecisionTypeId == DecisionCode.StatementReqBaptism):
        if (p.DiscClassStatusCompletedCodes.Contains(p.NewMemberClassStatusId)
		and p.BaptismStatusId == BaptismStatusCode.Completed):
            p.MemberStatusId = MemberStatusCode.Member
            p.JoinCodeId = JoinTypeCode.BaptismSRB
            if (p.NewMemberClassDate != None):
                 if (p.NewMemberClassDate > p.BaptismDate):
                    SetJoinDate(p, "NewMember", p.NewMemberClassDate)
                 else: 
                    SetJoinDate(p, "Baptism", p.BaptismDate)
            else:
                 SetJoinDate(p, "Baptism", p.BaptismDate)

# this moves the membership process along, setting various codes based on decision
def CheckDecisionStatus(p):

    if (p.DecisionTypeId == DecisionCode.ProfessionForMembership):
        p.MemberStatusId = MemberStatusCode.Pending
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
        if (p.Age <= 12 and p.FamilyHasPrimaryMemberForMoreThanDays(365)):
            p.BaptismTypeId = BaptismTypeCode.Biological
        else:
            p.BaptismTypeId = BaptismTypeCode.Original
        BaptismStatusId = BaptismStatusCode.NotScheduled

    elif (p.DecisionTypeId == DecisionCode.ProfessionNotForMembership):
        p.MemberStatusId = MemberStatusCode.NotMember
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            NewMemberClassStatusId = NewMemberClassStatusCode.NotSpecified
        if (p.BaptismStatusId != BaptismStatusCode.Completed):
            p.BaptismTypeId = BaptismTypeCode.NonMember
            p.BaptismStatusId = BaptismStatusCode.NotScheduled

    elif (p.DecisionTypeId == DecisionCode.Letter):
        p.MemberStatusId = MemberStatusCode.Pending
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
            if (p.BaptismStatusId != BaptismStatusCode.Completed):
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.NotSpecified

    elif (p.DecisionTypeId == DecisionCode.Statement):
        p.MemberStatusId = MemberStatusCode.Pending
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
            if (p.BaptismStatusId != BaptismStatusCode.Completed):
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.NotSpecified

    elif (p.DecisionTypeId == DecisionCode.StatementReqBaptism):
        p.MemberStatusId = MemberStatusCode.Pending
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            p.NewMemberClassStatusId = NewMemberClassStatusCode.Pending
        if (p.BaptismStatusId != BaptismStatusCode.Completed):
            p.BaptismTypeId = BaptismTypeCode.Required
            p.BaptismStatusId = BaptismStatusCode.NotScheduled

    elif (p.DecisionTypeId == DecisionCode.Cancelled):
        p.MemberStatusId = MemberStatusCode.NotMember
        if (p.NewMemberClassStatusId != NewMemberClassStatusCode.Attended):
            NewMemberClassStatusId = NewMemberClassStatusCode.NotSpecified
        if (p.BaptismStatusId != BaptismStatusCode.Completed):
            if (p.BaptismStatusId != BaptismStatusCode.Completed):
                p.BaptismTypeId = BaptismTypeCode.NotSpecified
                p.BaptismStatusId = BaptismStatusCode.Canceled
        p.EnvelopeOptionsId = EnvelopeOptionCode.NoEnvelope

# cleanup for deceased and for dropped memberships
def DropMembership(p, Db):

    if (p.MemberStatusId == MemberStatusCode.Member):
        if p.Deceased:
            p.DropCodeId = DropTypeCode.Deceased
        p.MemberStatusId = MemberStatusCode.Previous
        p.DropDate = p.Now().Date

    if p.Deceased:
        p.EmailAddress = None
        p.DoNotCallFlag = True
        p.DoNotMailFlag = True
        p.DoNotVisitFlag = True

    if p.SpouseId != None:
        spouse = DbUtil.Db.LoadPersonById(p.SpouseId)

        if p.Deceased:
            spouse.MaritalStatusId = MaritalStatusCode.Widowed
            if spouse.EnvelopeOptionsId != None: # not null
                if spouse.EnvelopeOptionsId != EnvelopeOptionCode.NoEnvelope:
                    spouse.EnvelopeOptionsId = EnvelopeOptionCode.Individual
            spouse.ContributionOptionsId = EnvelopeOptionCode.Individual

        if (spouse.MemberStatusId == MemberStatusCode.Member):
            if (spouse.EnvelopeOptionsId == EnvelopeOptionCode.Joint):
                spouse.EnvelopeOptionsId = EnvelopeOptionCode.Individual

    p.EnvelopeOptionsId = EnvelopeOptionCode.NoEnvelope
    p.DropAllMemberships(CurrentDatabase)

#-------------------------------------
# Main Function
class MembershipAutomation:
    def Run(self, Db, p):
        p.errorReturn = "ok"
        if p.DecisionTypeIdChanged:
            CheckDecisionStatus(p)

        if ((p.NewMemberClassStatusId == NewMemberClassStatusCode.AdminApproval
		or p.NewMemberClassStatusId == NewMemberClassStatusCode.Attended
		or p.NewMemberClassStatusId == NewMemberClassStatusCode.GrandFathered
		or p.NewMemberClassStatusId == NewMemberClassStatusCode.ExemptedChild
	      ) and p.NewMemberClassDate == None):
            p.errorReturn = "need a NewMemberClass date"

        if ((p.DecisionTypeId == DecisionCode.Letter
		or p.DecisionTypeId == DecisionCode.Statement 
		or p.DecisionTypeId == DecisionCode.ProfessionForMembership
		or p.DecisionTypeId == DecisionCode.ProfessionNotForMembership
		or p.DecisionTypeId == DecisionCode.StatementReqBaptism
		) and p.DecisionDate == None):
            p.errorReturn = "need a Decision date"

        if (p.NewMemberClassStatusIdChanged or p.BaptismStatusIdChanged):
            CheckJoinStatus(p)

        if p.DeceasedDateChanged:
            if p.DeceasedDate != None:
                DropMembership(p, Db)

        # when people leave the church, lots of cleanup to do
        if p.DropCodeIdChanged:
            if p.DropCodesThatDrop.Contains(p.DropCodeId):
                DropMembership(p, Db)

        om2 = DbUtil.Db.LoadOrgMember(p.PeopleId, "Step 1", False)
        if om2 != None:
            om2 = None


        # this does new member class completed
        if (p.NewMemberClassStatusIdChanged
		and p.NewMemberClassStatusId == NewMemberClassStatusCode.Attended):
            om = DbUtil.Db.LoadOrgMember(p.PeopleId, "Step 1", False)
            if om != None:
                om.Drop(DbUtil.Db, True) # drops and records drop in history
