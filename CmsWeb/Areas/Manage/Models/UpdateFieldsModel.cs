/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections;
using System.Linq;
using System.Web;
using CmsWeb.Code;
using UtilityExtensions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.Linq;
using CmsData;
using System.Collections.Generic;

namespace CmsWeb.Models
{
    public class UpdateFieldsModel
    {
        public string Tag { get; set; }
        public string Field { get; set; }
        public string NewValue { get; set; }

        public SelectList Fields()
        {
            return new SelectList(new[] { 
                "(not specified)",
                "Campus",
                "Drop Type",
                "Employer",
                "Family Position",
                "Entry Point",
                "Gender",
                "Grade",
                "Marital Status",
                "Member Status",
                "Baptism Status",
                "Baptism Type",
                "Join Type",
                "Statement Options",
                "Envelope Options",
                "Title",
                "Occupation",
                "School",
                "ReceiveSMS",
                "Background Check Date",
                "New Member Status",
                "Approval Codes",
                "Drop All Memberships",
                "Deceased Date",
            }.Select(x => new { value = x, text = x }),
                "value", "text");
        }
        public IEnumerable<SelectListItem> Tags()
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
            return tg;
        }
        public IEnumerable<TitleItems> TitleItems()
        {
            var Model = new CodeValueModel();
            return new List<TitleItems>
            {
                new TitleItems { title = "Member Status Codes", items = Model.MemberStatusCodes() },
                new TitleItems { title = "Drop Type Codes", items = Model.DropTypeList() },
                new TitleItems { title = "Baptism Status Codes", items = Model.BaptismStatusList() },
                new TitleItems { title = "Baptism Type Codes", items = Model.BaptismTypeList() },
                new TitleItems { title = "Join Type Codes", items = Model.JoinTypeList() },
                new TitleItems { title = "Campus Codes", items = Model.AllCampuses() },
                new TitleItems { title = "Marital Status Codes", items = Model.MaritalStatusCodes() },
                new TitleItems { title = "Gender Codes", items = Model.GenderCodes() },
                new TitleItems { title = "Family Position Codes", items = Model.FamilyPositionCodes() },
                new TitleItems { title = "Contribution Statement Options", items = Model.EnvelopeOptionList() },
                new TitleItems { title = "Envelope Options", items = Model.EnvelopeOptionList() },
                new TitleItems { title = "Entry Point Codes", items = Model.EntryPoints() },
                new TitleItems { title = "New Member Class", items = Model.NewMemberClassStatusList() },
                new TitleItems { title = "Approval Codes (negative to remove, 0 to remove all)", items = Model.VolunteerCodes() },
            };
        }
    }
    public class TitleItems
    {
        public string title { get; set; }
        public IEnumerable<CodeValueItem> items { get; set; }
    }
}
