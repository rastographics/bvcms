/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using System;
using System.Linq;
using System.Web;
using CmsWeb.Code;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Models
{
    public class UpdateFieldsModel
    {
        public string Tag { get; set; }
        public string Field { get; set; }
        public string NewValue { get; set; }

        public List<SelectListItem> Fields()
        {
            return new SelectList(new[] { 
                "(not specified)",
                "Approval Codes",
                "Background Check Date",
                "Bad Address Flag",
                "Baptism Status",
                "Baptism Type",
                "Campus",
                "Deceased Date",
                "Decision Type",
                "Do Not Mail",
                "Drop Type",
                "Drop All Enrollments",
                "Employer",
                "Entry Point",
                "Electronic Statement",
                "Envelope Options",
                "Family Position",
                "Gender",
                "Grade",
                "Join Type",
                "Marital Status",
                "Member Status",
                "New Member Status",
                "Occupation",
                "ReceiveSMS",
                "School",
                "Statement Options",
                "Title",
            }.Select(x => new { value = x, text = x }),
                "value", "text").ToList();
        }
        public List<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id");
            tg = tg.Select(tt => new SelectListItem { Text = "tag: {0}:{1}".Fmt(tt.Value, tt.Text) }).ToList();
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue != null
                    group e by e.FieldValue into g
                    select new SelectListItem { Text = "exval: " + g.Key };
            tg.AddRange(q);
            if (HttpContext.Current.User.IsInRole("Admin"))
                tg.Insert(0, new SelectListItem { Text = "last query" });
            tg.Insert(0, new SelectListItem { Text = "(not specified)" });
            var sel = tg.SingleOrDefault(mm => mm.Text == Tag);
            if (sel != null)
                sel.Selected = true;
            return tg.ToList();
        }
        public IEnumerable<TitleItems> FetchTitleItems()
        {
            var Model = new CodeValueModel();
            return new List<TitleItems>
            {
                new TitleItems { title = "Approval Codes", Instructions = "(negative to remove, 0 to remove all)", items = Model.VolunteerCodes() },
                new TitleItems { title = "Baptism Status Codes", items = Model.BaptismStatusList() },
                new TitleItems { title = "Baptism Type Codes", items = Model.BaptismTypeList() },
                new TitleItems { title = "Bad Address Flag", items = BadAddressFlag(), UseCode = true },
                new TitleItems { title = "Campus Codes", items = Model.AllCampuses() },
                new TitleItems { title = "Contribution Statement Options", items = Model.EnvelopeOptionList() },
                new TitleItems { title = "Electronic Statement", items = ElectronicStatement(), UseCode = true },
                new TitleItems { title = "Decision Type Codes", items = Model.DecisionTypeList() },
                new TitleItems { title = "Do Not Mail", items = DoNotMail(), UseCode = true },
                new TitleItems { title = "Drop Type Codes", items = Model.DropTypeList() },
                new TitleItems { title = "Envelope Options", items = Model.EnvelopeOptionList() },
                new TitleItems { title = "Entry Point Codes", items = Model.EntryPoints() },
                new TitleItems { title = "Family Position Codes", items = Model.FamilyPositionCodes() },
                new TitleItems { title = "Gender Codes", items = Model.GenderCodes() },
                new TitleItems { title = "Grades", items = Grades(), UseCode = true},
                new TitleItems { title = "Join Type Codes", items = Model.JoinTypeList() },
                new TitleItems { title = "Marital Status Codes", items = Model.MaritalStatusCodes() },
                new TitleItems { title = "Member Status Codes", items = Model.MemberStatusCodes() },
                new TitleItems { title = "New Member Class", items = Model.NewMemberClassStatusList() },
                new TitleItems { title = "Receive SMS", items = ReceiveSMS(), UseCode = true},
            };
        }

        public int? Count { get; set; }

        public IEnumerable<Person> People()
        {
            var a = Tag.SplitStr(":", 2);
            if (a.Length > 1)
                a[1] = a[1].TrimStart();
            IQueryable<Person> q = null;
            switch (a[0])
            {
                case "last query":
                    {
                        var id = DbUtil.Db.FetchLastQuery().Id;
                        q = DbUtil.Db.PeopleQuery(id);
                    }
                    break;
                case "tag":
                    {
                        var b = a[1].SplitStr(":", 2);
                        var tag = DbUtil.Db.TagById(b[0].ToInt());
                        q = tag.People(DbUtil.Db);
                    }
                    break;
                case "exval":
                    {
                        var b = a[1].SplitStr(":", 2);
                        q = from e in DbUtil.Db.PeopleExtras
                            where e.Field == b[0]
                            where e.StrValue == b[1]
                            select e.Person;
                    }
                    break;
            }
            Count = q.Count();
            return q;
        }

        private ModelStateDictionary modelState;
        public void Run(ModelStateDictionary model)
        {
            modelState = model;
            var q = People();
            if (q == null)
            {
                model.AddModelError("Tag", "Select a tag/query");
                return;
            }
            if (Field == "(not specified)")
                model.AddModelError("Field", "Select a Field");

            if (!model.IsValid)
                return;

            if (Field == "Bad Address Flag")
            {
                foreach (var f in q.Select(p => p.Family))
                {
                    f.BadAddressFlag = NewValue.ToBool();
                    DbUtil.Db.SubmitChanges();
                }
                return;
            }
            foreach (var p in q)
            {
                switch (Field)
                {
                    case "Approval Codes":
                        DoApprovalCodes(p);
                        break;
                    case "Background Check Date":
                        DoBackgroundChecks(p);
                        break;
                    case "Baptism Status":
                        p.BaptismStatusId = NewValue.ToInt2();
                        break;
                    case "Baptism Type":
                        p.BaptismTypeId = NewValue.ToInt2();
                        break;
                    case "Campus":
                        p.CampusId = NewValue.ToInt2();
                        break;
                    case "Deceased Date":
                        if (DateValid())
                            p.DeceasedDate = NewValue.ToDate();
                        break;
                    case "Decision Type":
                        p.DecisionTypeId = NewValue.ToInt2();
                        break;
                    case "Do Not Mail":
                        p.DoNotMailFlag = NewValue.ToBool();
                        break;
                    case "Drop All Enrollments":
                        p.DropMemberships(DbUtil.Db);
                        break;
                    case "Drop Type":
                        p.DropCodeId = NewValue.ToInt();
                        break;
                    case "Entry Point":
                        p.EntryPointId = NewValue.ToInt2();
                        break;
                    case "Employer":
                        p.EmployerOther = NewValue;
                        break;
                    case "Envelope Options":
                        p.EnvelopeOptionsId = NewValue.ToInt2();
                        break;
                    case "Family Position":
                        p.PositionInFamilyId = NewValue.ToInt();
                        break;
                    case "Gender":
                        p.GenderId = NewValue.ToInt();
                        break;
                    case "Grade":
                        DoGrade(p);
                        break;
                    case "Join Type":
                        p.JoinCodeId = NewValue.ToInt();
                        break;
                    case "Marital Status":
                        p.MaritalStatusId = NewValue.ToInt();
                        break;
                    case "Member Status":
                        p.MemberStatusId = NewValue.ToInt();
                        break;
                    case "Occupation":
                        p.OccupationOther = NewValue;
                        break;
                    case "New Member Class":
                        p.NewMemberClassStatusId = NewValue.ToInt2();
                        break;
                    case "ReceiveSMS":
                        p.ReceiveSMS = NewValue.ToBool();
                        break;
                    case "School":
                        p.SchoolOther = NewValue;
                        break;
                    case "Statement Options":
                        p.ContributionOptionsId = NewValue.ToInt2();
                        break;
                    case "Electronic Statement":
                        p.ElectronicStatement = NewValue.ToBool2();
                        break;
                    case "Title":
                        p.TitleCode = NewValue;
                        break;
                }
                if (!model.IsValid)
                    return;
                DbUtil.Db.SubmitChanges();
            }
        }

        private void DoGrade(Person p)
        {
            if (NewValue == "+1")
                p.Grade = p.Grade + 1;
            else
                p.Grade = NewValue.ToInt2();
        }

        private bool DateValid()
        {
            if (Util.DateValid(NewValue))
                modelState.AddModelError("NewValue", "Must be Date");
            return modelState.IsValid;
        }
        private void DoBackgroundChecks(Person p)
        {
            if (!DateValid())
                return;
            var i = p.Volunteers.SingleOrDefault();
            if (i == null)
            {
                i = new Volunteer { PeopleId = p.PeopleId };
                DbUtil.Db.Volunteers.InsertOnSubmit(i);
                DbUtil.Db.SubmitChanges();
            }
            i.ProcessedDate = NewValue.ToDate();
        }

        private void DoApprovalCodes(Person p)
        {
            if (!NewValue.HasValue())
            {
                modelState.AddModelError("NewValue", "Must not be blank");
                return;
            }

            var i = p.Volunteers.SingleOrDefault();
            if (i == null)
            {
                i = new Volunteer { PeopleId = p.PeopleId };
                DbUtil.Db.Volunteers.InsertOnSubmit(i);
                DbUtil.Db.SubmitChanges();
            }

            var code = NewValue.ToInt();
            var app = (from v in DbUtil.Db.VoluteerApprovalIds
                       where v.ApprovalId == Math.Abs(code)
                       where v.PeopleId == p.PeopleId
                       select v).SingleOrDefault();
            if (code < 0)
            {
                if (app != null)
                    DbUtil.Db.VoluteerApprovalIds.DeleteOnSubmit(app);
            }
            else if (code > 0)
            {
                if (app == null)
                    p.VoluteerApprovalIds.Add(new VoluteerApprovalId { ApprovalId = code, });
            }
            else if (code == 0)
            {
                DbUtil.Db.VoluteerApprovalIds.DeleteAllOnSubmit(p.VoluteerApprovalIds);
            }
        }

        public class TitleItems
        {
            public string title { get; set; }
            public bool UseCode { get; set; }
            public IEnumerable<CodeValueItem> items { get; set; }
            public string Instructions { get; set; }
        }
        public static List<CodeValueItem> Grades()
        {
            return new List<CodeValueItem> 
				{
					new CodeValueItem { Code = "-1", Value = "Pre K" },
					new CodeValueItem { Code = "0", Value = "Kindergarten" },
					new CodeValueItem { Code = "1-12", Value = "Grade" },
					new CodeValueItem { Code = "13", Value = "Freshman" },
					new CodeValueItem { Code = "14", Value = "Sophmore" },
					new CodeValueItem { Code = "15", Value = "Junior" },
					new CodeValueItem { Code = "16", Value = "Senior" },
					new CodeValueItem { Code = "99", Value = "Special Class" },
					new CodeValueItem { Code = "YYYY", Value = "Graduation Year" },
					new CodeValueItem { Code = "+1", Value = "Add 1 to the Grade" },
				};
        }
        public static List<CodeValueItem> ReceiveSMS()
        {
            return new List<CodeValueItem> 
				{
					new CodeValueItem { Code = "true", Value = "Yes, Receive SMS Text messages" },
					new CodeValueItem { Code = "false", Value = "No, Do not Receive SMS Text messages" },
				};
        }
        public static List<CodeValueItem> BadAddressFlag()
        {
            return new List<CodeValueItem> 
				{
					new CodeValueItem { Code = "true", Value = "Yes, Address is Bad" },
					new CodeValueItem { Code = "false", Value = "No, Address is Good" },
				};
        }
        public static List<CodeValueItem> DoNotMail()
        {
            return new List<CodeValueItem> 
				{
					new CodeValueItem { Code = "true", Value = "Yes, Do Not Mail" },
					new CodeValueItem { Code = "false", Value = "No, OK To Mail" },
				};
        }
        public static List<CodeValueItem> ElectronicStatement()
        {
            return new List<CodeValueItem> 
				{
					new CodeValueItem { Code = "true", Value = "Yes, Only Electronic Statements" },
					new CodeValueItem { Code = "false", Value = "No, Paper Statements" },
				};
        }
    }
}
