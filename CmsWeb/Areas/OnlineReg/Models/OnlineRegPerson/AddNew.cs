using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public bool PrepareToAddNewPerson(ModelStateDictionary modelstate, int id)
        {
            modelState = modelstate;
            Index = id;

            Parent.HistoryAdd("ShowMoreInfo id=" + id);

            if (org != null && Found == true)
            {
                if (!Parent.SupportMissionTrip)
                    IsFilled = org.RegLimitCount(DbUtil.Db) >= org.Limit;
                if (IsFilled)
                    modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].DateOfBirth), "Sorry, but registration is closed.");
                if (Found == true)
                    FillPriorInfo();
                return true;
            }
//            if (!whatfamily.HasValue && (id > 0 || Parent.UserPeopleId > 0 ))
//            {
//                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].whatfamily), "Choose a family option");
//                return true;
//            }
            switch (whatfamily)
            {
                case 1:
                    Debug.Assert(Parent.UserPeopleId != null);
                    var u = DbUtil.Db.LoadPersonById(Parent.UserPeopleId.Value);
                    AddressLineOne = u.PrimaryAddress;
                    City = u.PrimaryCity;
                    State = u.PrimaryState;
                    ZipCode = u.PrimaryZip.FmtZip();
                    break;
                case 2:
                    var pb = Parent.List[id - 1];
                    AddressLineOne = pb.AddressLineOne;
                    City = pb.City;
                    State = pb.State;
                    ZipCode = pb.ZipCode;
                    break;
                default:
#if DEBUG2
                    AddressLineOne = "235 Riveredge Cv.";
                    City = "Cordova";
                    State = "TN";
                    ZipCode = "38018";
                    gender = 1;
                    married = 10;
                    HomePhone = "9017581862";
#endif
                    break;
            }
            whatfamily = 3;
            ShowAddress = true;
            return false;
        }

        public string AddNew(ModelStateDictionary modelstate, int id)
        {
            modelState = modelstate;
            Index = id;

            if (modelState.IsValid)
            {
                if (Parent.ManagingSubscriptions())
                {
                    IsNew = true;
                    Parent.ConfirmManageSubscriptions();
                    DbUtil.Db.SubmitChanges();
                    return "ManageSubscriptions/OneTimeLink";
                }
                if (Parent.OnlinePledge())
                {
                    IsNew = true;
                    Parent.SendLinkForPledge();
                    DbUtil.Db.SubmitChanges();
                    return "ManagePledge/OneTimeLink";
                }
                if (Parent.ManageGiving())
                {
                    IsNew = true;
                    Parent.SendLinkToManageGiving();
                    DbUtil.Db.SubmitChanges();
                    return "ManageGiving/OneTimeLink";
                }
                if (ComputesOrganizationByAge())
                {
                    if (org == null)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].Found), "Sorry, cannot find an appropriate age group");
                    else if (org.RegEnd.HasValue && DateTime.Now > org.RegEnd)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].Found), "Sorry, registration has ended for that group");
                    else if (org.OrganizationStatusId == OrgStatusCode.Inactive)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].Found), "Sorry, that group is inactive");
                    else if (org.OrganizationStatusId == OrgStatusCode.Inactive)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].Found), "Sorry, that group is inactive");
                }
                else if (!ManageSubscriptions())
                {
                    if (!Parent.SupportMissionTrip)
                        IsFilled = org.RegLimitCount(DbUtil.Db) >= org.Limit;
                    if (IsFilled)
                        modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].Found), "Sorry, registration is filled");
                }
                IsNew = true;
            }
            IsValidForExisting = modelState.IsValid == false;
            if (IsNew)
                FillPriorInfo();
            if (org != null && FinishedFindingOrAddingRegistrant && ComputesOrganizationByAge())
                classid = org.OrganizationId;
            return null;
        }
        internal bool AnonymousReRegistrant()
        {
            if (Found != true || Parent.Orgid == null) 
                return false;
            if (!setting.AllowReRegister) 
                return false;
            var om = Parent.org.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == PeopleId);
            if (om == null) 
                return false;

            Parent.ConfirmReregister();
            DbUtil.Db.SubmitChanges();
            return true;
        }
    }
}