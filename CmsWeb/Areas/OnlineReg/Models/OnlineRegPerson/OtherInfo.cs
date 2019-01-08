using CmsData;
using CmsData.Finance;
using CmsData.Registration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        private List<string> groupTags;

        public List<string> GroupTags
        {
            get
            {
                if (groupTags == null)
                {
                    groupTags = (from mt in DbUtil.Db.OrgMemMemTags
                                 where mt.OrgId == org.OrganizationId
                                 select mt.MemberTag.Name).ToList();
                }

                var gtdd = (from pp in Parent.List
                            where pp != this
                            where pp.option != null
                            from oo in pp.option
                            where oo.HasValue()
                            select oo).ToList();
                var gtcb = (from pp in Parent.List
                            where pp != this
                            where pp.Checkbox != null
                            from cc in pp.Checkbox
                            where cc.HasValue()
                            select cc).ToList();
                var r = new List<string>();
                r.AddRange(groupTags);
                r.AddRange(gtdd);
                r.AddRange(gtcb);
                return r;
            }
        }

        public string ExtraQuestionValue(int set, string s)
        {
            if (set >= ExtraQuestion.Count)
            {
                return null;
            }

            if (ExtraQuestion[set].ContainsKey(s))
            {
                return ExtraQuestion[set][s];
            }

            return null;
        }

        public string TextValue(int set, string s)
        {
            if (set >= Text.Count)
            {
                return null;
            }

            if (Text[set].ContainsKey(s))
            {
                return Text[set][s];
            }

            return null;
        }

        public bool Attended(int id)
        {
            var a = FamilyAttend?.SingleOrDefault(aa => aa.PeopleId == id);
            if (a == null)
            {
                return false;
            }

            return a.Attend;
        }

        public bool YesNoChecked(string key, bool value)
        {
            if (YesNoQuestion != null && YesNoQuestion.ContainsKey(key))
            {
                return YesNoQuestion[key] == value;
            }

            return false;
        }

        public bool CheckboxChecked(string sg)
        {
            if (Checkbox == null)
            {
                return false;
            }

            return Checkbox.Contains(sg);
        }

        public IEnumerable<SelectListItemFilled> DropdownList(Ask ask)
        {
            // this appears to only occur when a user saves progress, the organization has a dropdown question added, and then the user continues
            // we need to ensure that we have options set for all of the questions
            while (ask.UniqueId >= option.Count)
            {
                option.Add(string.Empty);
            }

            var q = from s in ((AskDropdown)ask).list
                    let amt = s.Fee.HasValue ? $" ({s.Fee:C})" : ""
                    select new SelectListItemFilled
                    {
                        Text = s.Description + amt,
                        Value = s.SmallGroup,
                        Filled = s.IsSmallGroupFilled(GroupTags),
                        Selected = s.SmallGroup == option[ask.UniqueId]
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItemFilled { Text = "(please select)", Value = "00" });
            return list;
        }

        public bool RetrieveEntireFundList { get; set; }

        public IEnumerable<GivingConfirmation.FundItem> FundItemsChosen()
        {
            if (FundItem == null)
            {
                return new List<GivingConfirmation.FundItem>();
            }

            var items = RetrieveEntireFundList ? EntireFundList() : AllFunds();
            var q = from i in FundItem
                    join m in items on i.Key equals m.Value.ToInt()
                    where i.Value.HasValue
                    select new GivingConfirmation.FundItem() { Fundid = m.Value.ToInt(), Desc = m.Text, Amt = i.Value ?? 0 };
            return q;
        }

        public IEnumerable<SelectListItem> GradeOptions(Ask ask)
        {
            var q = from s in ((AskGradeOptions)ask).list
                    select new SelectListItem { Text = s.Description, Value = s.Code.ToString() };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(please select)", Value = "00" });
            return list;
        }

        public static IEnumerable<SelectListItem> ShirtSizes(CMSDataContext Db, Organization org)
        {
            var setting = DbUtil.Db.CreateRegistrationSettings(org.OrganizationId);
            return ShirtSizes(setting);
        }

        private static IEnumerable<SelectListItem> ShirtSizes(Settings setting)
        {
            var list = new List<SelectListItem>();
            list.Insert(0, new SelectListItem { Value = "0", Text = "(please select)" });
            var askSize = setting.AskItems.FirstOrDefault(aa => aa is AskSize) as AskSize;
            if (askSize != null)
            {
                var q = from ss in askSize.list
                        select new SelectListItem
                        {
                            Value = ss.SmallGroup,
                            Text = ss.Description
                        };
                list.InsertRange(1, q.ToList());
            }
            if (askSize?.AllowLastYear ?? false)
            {
                var text = Util.PickFirst(Organization.GetExtra(DbUtil.Db, setting.OrgId, "AllowLastYearShirtText"),
                    "Use shirt from last year");
                list.Add(new SelectListItem { Value = "lastyear", Text = text });
            }
            return list;
        }

        public IEnumerable<SelectListItem> ShirtSizes()
        {
            return ShirtSizes(setting);
        }

        public List<SelectListItem> MissionTripGoers()
        {
            var pid = person?.PeopleId;
            var q = from g in DbUtil.Db.OrganizationMembers
                    where g.OrganizationId == orgid
                    where g.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer")
                    where g.PeopleId != (Parent.UserPeopleId ?? pid ?? 0)
                    orderby g.Person.Name2
                    select new SelectListItem
                    {
                        Value = g.PeopleId.ToString(),
                        Text = g.Person.Name2
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "0", Text = "(please select)" });
            return list;
        }
        public bool IsGoer()
        {
            var pid = person?.PeopleId ?? 0;
            return DbUtil.Db.OrganizationMembers.Any(mm => mm.OrganizationId == orgid && mm.PeopleId == pid);
        }

        public void FillPriorInfo()
        {
            if (Found != true)
            {
                return;
            }

            if (IsNew || !LoggedIn)
            {
                return;
            }

            var rr = DbUtil.Db.RecRegs.SingleOrDefault(r => r.PeopleId == PeopleId);
            if (rr != null)
            {
                if (setting.AskVisible("AskRequest"))
                {
                    var om = GetOrgMember();
                    if (om != null)
                    {
                        request = om.Request;
                    }
                }
                if (setting.AskVisible("AskSize"))
                {
                    shirtsize = rr.ShirtSize;
                }

                if (setting.AskVisible("AskEmContact"))
                {
                    emcontact = rr.Emcontact;
                    emphone = rr.Emphone;
                }
                if (setting.AskVisible("AskInsurance"))
                {
                    insurance = rr.Insurance;
                    policy = rr.Policy;
                }
                if (setting.AskVisible("AskDoctor"))
                {
                    docphone = rr.Docphone;
                    doctor = rr.Doctor;
                }
                if (setting.AskVisible("AskParents"))
                {
                    mname = rr.Mname;
                    fname = rr.Fname;
                }
                if (setting.AskVisible("AskAllergies"))
                {
                    medical = rr.MedicalDescription;
                }

                if (setting.AskVisible("AskCoaching"))
                {
                    coaching = rr.Coaching;
                }

                if (setting.AskVisible("AskChurch"))
                {
                    otherchurch = rr.ActiveInAnotherChurch ?? false;
                    memberus = rr.Member ?? false;
                }
                if (setting.AskVisible("AskTylenolEtc"))
                {
                    tylenol = rr.Tylenol;
                    advil = rr.Advil;
                    robitussin = rr.Robitussin;
                    maalox = rr.Maalox;
                }
            }
#if DEBUG2
            request = "Toby";
            ntickets = 1;
            gradeoption = "12";
            YesNoQuestion["Facebook"] = true;
            YesNoQuestion["Twitter"] = true;
            ExtraQuestion["Your Occupation"] = "programmer";
            ExtraQuestion["Your Favorite Snack"] = "peanuts";
            MenuItem["Fish"] = 1;
            MenuItem["Turkey"] = 0;
            option = "opt2";
            option2 = "none";
            paydeposit = false;
            Checkbox = new string[] { "PuttPutt", "Horseshoes" };
            shirtsize = "XL";
            emcontact = "dc";
            emphone = "br545";
            insurance = "bcbs";
            policy = "2424";
            doctor = "costalot";
            docphone = "35353365";
            tylenol = true;
            advil = true;
            maalox = false;
            robitussin = false;
            fname = "david carroll";
            coaching = false;
            paydeposit = false;
            grade = "4";
#endif
        }

        public bool NeedsCopyFromPrevious()
        {
            if (org != null)
            {
                return (setting.AskVisible("AskEmContact")
                        || setting.AskVisible("AskInsurance")
                        || setting.AskVisible("AskDoctor")
                        || setting.AskVisible("AskParents"));
            }

            return false;
        }

        public class SelectListItemFilled : SelectListItem
        {
            public bool Filled { get; set; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class FundItemChosen
        {
            public string desc { get; set; }
            public int fundid { get; set; }
            public decimal amt { get; set; }
        }
    }
}
