using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Models
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
            if (!whatfamily.HasValue && (id > 0 || LoggedIn == true))
            {
                modelState.AddModelError(Parent.GetNameFor(mm => mm.List[id].whatfamily), "Choose a family option");
                return true;
            }
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
#if DEBUG
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
            ShowAddress = true;
            return false;
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