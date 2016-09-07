/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Areas.Search.Controllers;
using CmsWeb.Models;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class PictureDirectoryModel : PagedTableModel<Person, dynamic>
    {
        private readonly string flag;
        private readonly bool hasAccess;
        public readonly bool CanView;
        private string orderby;

        public PictureDirectoryModel()
            : base("Name", "asc", true)
        {
            flag = DbUtil.Db.Setting("PictureDirectoryStatusFlag", "");
            hasAccess = HttpContext.Current.User.IsInRole("Access");
            var hasstatus = (from v in DbUtil.Db.StatusFlags(flag)
                             where v.PeopleId == Util.UserPeopleId
                             select v).Any();
            CanView = hasstatus || HttpContext.Current.User.IsInRole("Admin");
            GetCount = CountEntries;
        }

        public string Name { get; set; }
        public bool FamilyOption { get; set; }

        private List<dynamic> entries;
        public IEnumerable<dynamic> Entry => entries ?? (entries = ViewList().ToList());

        public int CountEntries()
        {
            if (!count.HasValue)
                count = DefineModelList().Count();
            return count.Value;
        }

        public override IQueryable<Person> DefineModelList()
        {
            IQueryable<Person> qmembers;
            if (!CanView)
                qmembers = DbUtil.Db.PeopleQuery2($"PeopleId = {Util.UserPeopleId}");
            else
            {
                var qs = $"StatusFlag = '{flag}'";
                qmembers = DbUtil.Db.PeopleQuery2(qs);
            }

            if (Name.HasValue())
                qmembers = from p in qmembers
                           where p.Name.Contains(Name)
                           select p;
            return qmembers;
        }

        public override IQueryable<Person> DefineModelSort(IQueryable<Person> q)
        {
            if (Direction == "asc")
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Name2
                            select p;
                        orderby = "ORDER BY p.Name2";
                        break;
                    case "Birthday":
                        q = from p in q
                            orderby DbUtil.Db.NextBirthday(p.PeopleId)
                            select p;
                        orderby = "ORDER BY dbo.NextBirthday(PeopleId)";
                        break;
                }
            else
            {
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Name2 descending
                            select p;
                        orderby = "ORDER BY p.Name2 DESC";
                        break;
                    case "Birthday":
                        q = from p in q
                            orderby DbUtil.Db.NextBirthday(p.PeopleId) descending
                            select p;
                        orderby = "ORDER BY dbo.NextBirthday(PeopleId) DESC";
                        break;
                }
            }
            return q;
        }

        public override IEnumerable<dynamic> DefineViewList(IQueryable<Person> q)
        {
            var tagid = DbUtil.Db.PopulateTemporaryTag(q.Select(vv => vv.PeopleId)).Id;
            var qf = new QueryFunctions(DbUtil.Db);
            var sq = qf.QuerySql(
                $"{DbUtil.Db.ContentSql("PictureDirectory", Resource1.PictureDirectorySql)}\n{@orderby}", 
                tagid);
            return sq;
        }

        public string Results(PictureDirectoryController ctl)
        {
            try
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
                Handlebars.RegisterHelper("ImagePos", (w, ctx, args) =>
                {
                    var x = ctx.X;
                    var y = ctx.Y;
                    w.Write(x != null || y != null ? $"{x ?? 0}% {y ?? 0}%" : "top");
                });
                Handlebars.RegisterHelper("IfAccess", (w, opt, ctx, args) =>
                {
                    if (hasAccess)
                        opt.Template(w, (object)ctx);
                    else
                        opt.Inverse(w, (object)ctx);
                });
                Handlebars.RegisterHelper("PagerTop", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerTop", this)); });
                Handlebars.RegisterHelper("PagerBottom", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerBottom", this)); });
                Handlebars.RegisterHelper("PagerHidden", (w, ctx, args) => { w.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerHidden", this)); });
                Handlebars.RegisterHelper("SortBirthday", (w, ctx, args) => { w.Write(SortLink("Birthday")); });
                Handlebars.RegisterHelper("SortName", (w, ctx, args) => { w.Write(SortLink("Name")); });
                Handlebars.RegisterHelper("CSZ", (w, ctx, args) => { w.Write(Util.FormatCSZ4(args[0], args[1], args[2])); });
                var template = Handlebars.Compile(DbUtil.Db.ContentText("PictureDirectoryTemplate", Resource1.PictureDirectoryTemplate));
                var s = template(this);
                return s;
            }
            catch (Exception ex)
            {
                return ex.Message + "<br>\n" + ex.StackTrace;
            }
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