/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Search.Controllers;
using CmsWeb.Models;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class PictureDirectoryModel : PagedTableModel<Person, dynamic>
    {
        public bool HasAccess { get; set; }
        public bool? CanView { get; set; }
        public string TemplateName { get; set; }
        public string SqlName { get; set; }
        public string StatusFlag { get; set; }
        public int? OrgId { get; set; }
        public int? DivId { get; set; }
        public string Selector { get; set; }
        public string Url { get; set; }

        private string template;
        private string sql;
        private string orderBy;

        private const string PictureDirectoryTemplateName = "PictureDirectoryTemplate";
        private const string PictureDirectorySqlName = "PictureDirectorySql";

        public PictureDirectoryModel()
        {
        }

        public PictureDirectoryModel(string id)
            : base("Name", "asc")
        {
            Url = $"/PictureDirectory/{id}";
            if (id.HasValue())
            {
                if (Regex.IsMatch(id, @"\Ad\d+\z", RegexOptions.IgnoreCase))
                {
                    DivId = id.Substring(1).ToInt();
                }
                else if (id.GetDigits() == id)
                {
                    OrgId = id.ToInt();
                }
            }

            Initialize();
        }

        public void Initialize()
        {
            AjaxPager = true;
            HasAccess = HttpContextFactory.Current.User.IsInRole("Access");

            if (!Selector.HasValue())
            {
                GetConfiguration();
            }

            if (ErrorMessage.HasValue())
            {
                return;
            }

            template = GetModifiedOrLatestText(TemplateName);
            sql = GetModifiedOrLatestSql(SqlName);
        }

        private void GetConfiguration()
        {
            Selector = DbUtil.Db.Setting("PictureDirectorySelector", "");
            if (OrgId.HasValue)
            {
                Selector = "FromUrl";
            }
            else if (DivId.HasValue)
            {
                Selector = "FromUrl";
            }
            else if (Regex.IsMatch(Selector, @"\AF\d\d\z"))
            {
                StatusFlag = Selector;
            }
            else if (Regex.IsMatch(Selector, @"\A\d+\z"))
            {
                OrgId = Selector.ToInt();
            }

            if (OrgId.HasValue)
            {
                if (!CanView.HasValue)
                {
                    CanView = HttpContextFactory.Current.User.IsInRole("Admin") || DbUtil.Db.PeopleQuery2($@"
IsMemberOfDirectory( Org={OrgId} ) = 1 
AND PeopleId = {Util.UserPeopleId}", fromDirectory: true).Any();
                }
                //HasDirectory = (om.Organization.PublishDirectory ?? 0) > 0,

                TemplateName = Util.PickFirst(
                    Organization.GetExtra(DbUtil.Db, OrgId.Value, PictureDirectoryTemplateName),
                    PictureDirectoryTemplateName);
                SqlName = Util.PickFirst(
                    Organization.GetExtra(DbUtil.Db, OrgId.Value, PictureDirectorySqlName),
                    PictureDirectorySqlName);
            }
            else if (DivId.HasValue)
            {
                if (!CanView.HasValue)
                {
                    CanView = HttpContextFactory.Current.User.IsInRole("Admin") || DbUtil.Db.PeopleQuery2($@"
IsMemberOfDirectory( Div={DivId} ) = 1 
AND PeopleId = {Util.UserPeopleId}", fromDirectory: true).Any();
                }

                TemplateName = PictureDirectoryTemplateName + "-" + Selector;
                if (!DbUtil.Db.Contents.Any(vv => vv.Name == TemplateName && vv.TypeID == ContentTypeCode.TypeText))
                {
                    TemplateName = PictureDirectoryTemplateName;
                }

                SqlName = PictureDirectorySqlName + "-" + Selector;
                if (!DbUtil.Db.Contents.Any(vv => vv.Name == TemplateName && vv.TypeID == ContentTypeCode.TypeSqlScript))
                {
                    TemplateName = PictureDirectoryTemplateName;
                }
            }
            else if (StatusFlag.HasValue())
            {
                if (!CanView.HasValue)
                {
                    var hasstatus = (from v in DbUtil.Db.StatusFlags(StatusFlag)
                                     where v.PeopleId == Util.UserPeopleId
                                     where v.StatusFlags != null
                                     select v).Any();
                    CanView = hasstatus || HttpContextFactory.Current.User.IsInRole("Admin");
                }
                TemplateName = DbUtil.Db.Setting(PictureDirectoryTemplateName, PictureDirectoryTemplateName);
                SqlName = DbUtil.Db.Setting(PictureDirectorySqlName, PictureDirectorySqlName);
            }
            else
            {
                ErrorMessage = "NotConfigured";
            }

            if (!ErrorMessage.HasValue() && CanView == false)
            {
                ErrorMessage = "NotAuthorized";
            }
        }

        public static string GetModifiedOrLatestText(string name)
        {
            var s = DbUtil.Db.ContentOfTypeText(name);
            var re = new Regex(@"\A<!--default\.v(?<ver>\d+)");
            var m = re.Match(s);
            var ver = m.Groups["ver"].Value.ToInt();
            var currver = re.Match(Resource1.PictureDirectoryTemplate).Groups["ver"].Value.ToInt();
            if (!s.HasValue() || (m.Success && currver > ver))
            {
                s = DbUtil.Db.ContentText(name, Resource1.PictureDirectoryTemplate);
            }

            return s;
        }

        private static string GetModifiedOrLatestSql(string name)
        {
            var s = DbUtil.Db.ContentOfTypeSql(name);
            var re = new Regex(@"\A--default\.v(?<ver>\d+)");
            var m = re.Match(s);
            var ver = m.Groups["ver"].Value.ToInt();
            var currver = re.Match(Resource1.PictureDirectorySql).Groups["ver"].Value.ToInt();
            if (!s.HasValue() || (m.Success && currver > ver))
            {
                s = DbUtil.Db.ContentSql(name, Resource1.PictureDirectorySql);
            }

            return s;
        }

        public string Name { get; set; }
        public bool FamilyOption { get; set; }

        private List<dynamic> entries;
        public IEnumerable<dynamic> Entry => entries ?? (entries = ViewList().ToList());

        public string ErrorMessage { get; set; }

        public override IQueryable<Person> DefineModelList()
        {
            IQueryable<Person> qmembers;
            if (CanView != true)
            {
                qmembers = DbUtil.Db.PeopleQuery2("PeopleId = 0");
            }
            else if (StatusFlag.HasValue())
            {
                qmembers = DbUtil.Db.PeopleQuery2($"StatusFlag = '{StatusFlag}'", fromDirectory: true);
            }
            else if (OrgId.HasValue)
            {
                qmembers = DbUtil.Db.PeopleQuery2($"IsMemberOfDirectory( Org={OrgId} ) = 1 ", fromDirectory: true);
            }
            else if (DivId.HasValue)
            {
                qmembers = DbUtil.Db.PeopleQuery2($"IsMemberOfDirectory( Div={DivId} ) = 1", fromDirectory: true);
            }
            else
            {
                qmembers = DbUtil.Db.PeopleQuery2("PeopleId = 0");
            }

            if (Name.HasValue())
            {
                qmembers = from p in qmembers
                           where p.Family.HeadOfHousehold.LastName.Contains(Name) || p.Name.Contains(Name)
                           select p;
            }

            return qmembers;
        }

        public override IQueryable<Person> DefineModelSort(IQueryable<Person> q)
        {
            if (Direction == "asc")
            {
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Family.HeadOfHousehold.LastName,
                            p.FamilyId,
                            p.PositionInFamilyId,
                            p.PeopleId == p.Family.HeadOfHouseholdId ? 0 : 1,
                            p.Age descending
                            select p;
                        orderBy = "ORDER BY hh.LastName, p.FamilyId, p.PositionInFamilyId, IIF(p.PeopleId = hh.PeopleId, 0, 1), p.Age DESC";
                        break;
                    case "Birthday":
                        q = from p in q
                            orderby DbUtil.Db.NextBirthday(p.PeopleId)
                            select p;
                        orderBy = "ORDER BY dbo.NextBirthday(p.PeopleId)";
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Family.HeadOfHousehold.LastName descending,
                            p.FamilyId,
                            p.PositionInFamilyId,
                            p.PeopleId == p.Family.HeadOfHouseholdId ? 0 : 1,
                            p.Age descending
                            select p;
                        orderBy = "ORDER BY hh.LastName DESC, p.FamilyId, p.PositionInFamilyId, IIF(p.PeopleId = hh.PeopleId, 0, 1), p.Age DESC";
                        break;
                    case "Birthday":
                        q = from p in q
                            orderby DbUtil.Db.NextBirthday(p.PeopleId) descending
                            select p;
                        orderBy = "ORDER BY dbo.NextBirthday(p.PeopleId) DESC";
                        break;
                }
            }
            return q;
        }

        public override IEnumerable<dynamic> DefineViewList(IQueryable<Person> q)
        {
            var tagid = DbUtil.Db.PopulateTemporaryTag(q.Select(vv => vv.PeopleId)).Id;
            sql = sql.Replace("@p1", tagid.ToString());
            var qf = new QueryFunctions(DbUtil.Db);
            return qf.QuerySql($"{sql}\n{orderBy}", tagid);
        }

        public string Results(PictureDirectoryController ctl)
        {
            try
            {
                RegisterHelpers(ctl);
                var compiledTemplate = Handlebars.Compile(template);
                var s = compiledTemplate(this);
                return s;
            }
            catch (Exception ex)
            {
                return ex.Message + "<br>\n" + ex.StackTrace;
            }
        }

        private void RegisterHelpers(PictureDirectoryController ctl)
        {
            PythonModel.RegisterHelpers(DbUtil.Db);
            Handlebars.RegisterHelper("SmallUrl", (w, ctx, args) =>
            {
                GetPictureUrl(ctx, w, ctx.SmallId,
                    Picture.SmallMissingMaleId, Picture.SmallMissingFemaleId, Picture.SmallMissingGenericId);
            });
            Handlebars.RegisterHelper("MediumUrl", (w, ctx, args) =>
            {
                GetPictureUrl(ctx, w, ctx.MediumId,
                    Picture.MediumMissingMaleId, Picture.MediumMissingFemaleId, Picture.MediumMissingGenericId);
            });
            Handlebars.RegisterHelper("ImagePos", (w, ctx, args) => { w.Write(ctx.X != null || ctx.Y != null ? $"{ctx.X ?? 0}% {ctx.Y ?? 0}%" : "top"); });
            Handlebars.RegisterHelper("IfAccess", (w, opt, ctx, args) =>
            {
                if (HasAccess)
                {
                    opt.Template(w, (object)ctx);
                }
                else
                {
                    opt.Inverse(w, (object)ctx);
                }
            });
            Handlebars.RegisterHelper("PagerTop", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerTop", this)); });
            Handlebars.RegisterHelper("PagerBottom", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerBottom", this)); });
            Handlebars.RegisterHelper("PagerHidden", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerHidden", this)); });
            Handlebars.RegisterHelper("SortBirthday", (w, ctx, args) => { w.Write(SortLink("Birthday")); });
            Handlebars.RegisterHelper("SortName", (w, ctx, args) => { w.Write(SortLink("Name")); });
            Handlebars.RegisterHelper("CityStateZip", (w, ctx, args) => { w.Write(Util.FormatCSZ4(ctx.City, ctx.St, ctx.Zip)); });
            Handlebars.RegisterHelper("BirthDay", (w, ctx, args) =>
            {
                var dob = (string)ctx.DOB;
                w.Write(dob.ToDate().ToString2("m"));
            });
        }

        private static void GetPictureUrl(dynamic ctx, TextWriter w, int? id,
            int missingMaleId, int missingFemailId, int missingGenericId)
        {
            var genderid = ctx.GenderId;
            var created = ctx.PicDate;
            var missingid = genderid == 1 ? missingMaleId : genderid == 2 ? missingFemailId : missingGenericId;
            w.Write($"/Portrait/{id ?? missingid}?v={created?.Ticks ?? 0}");
        }
    }
}
