using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Models
{
    public class DashboardWidgetModel : IDbBinder
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int HTMLContentId { get; set; }
        public int PythonContentId { get; set; }
        public int SQLContentId { get; set; }
        public int[] Roles { get; set; }
        public bool Enabled;
        public int Order;
        public bool System;
        public int CachePolicy { get; set; }
        public int CacheHours { get; set; }
        public DateTime? CacheExpires;
        public string CachedContent;
        
        public Content HTMLContent { get; set; }
        public Content PythonContent { get; set; }
        public Content SQLContent { get; set; }

        public CMSDataContext CurrentDatabase { get; set; }
        private Content BlankContent = new Content { Id = 0, Name = "(not specified)" };

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public DashboardWidgetModel() { }
        public DashboardWidgetModel(CMSDataContext db)
        {
            CurrentDatabase = db;
        }

        public DashboardWidgetModel(string id, CMSDataContext db) {
            CurrentDatabase = db;
            DashboardWidget widget;
            if (id.ToLower() == "new")
            {
                Id = 0;
                widget = new DashboardWidget();
            }
            else
            {
                Id = id.ToInt();
                widget = CurrentDatabase.DashboardWidgets.Where(w => w.Id == id.ToInt()).Single();
            }
            this.Fill(widget);
        }
        
        public IEnumerable<SelectListItem> HtmlContents()
        {
            var Contents = CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypeText)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
            Contents.Add(BlankContent);
            return new SelectList(Contents, "Id", "Name", HTMLContentId);
        }

        public IEnumerable<SelectListItem> SQLContents()
        {
            var Contents = CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypeSqlScript)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
            Contents.Add(BlankContent);
            return new SelectList(Contents, "Id", "Name", SQLContentId);
        }

        public IEnumerable<SelectListItem> PythonContents()
        {
            var Contents = CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypePythonScript)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
            Contents.Add(BlankContent);
            return new SelectList(Contents, "Id", "Name", PythonContentId);
        }

        public IEnumerable<SelectListItem> AllRoles()
        {
            var AllRoles = CurrentDatabase.Roles
                    .OrderBy(r => r.RoleName)
                    .ToList();
            return new SelectList(AllRoles, "RoleId", "RoleName");
        }

        public IEnumerable<SelectListItem> CacheTimes()
        {
            var options = new[]
            {
                new { Value = 0, Name = "every page load" },
                new { Value = 1, Name = "every hour" },
                new { Value = 2, Name = "every 2 hours" },
                new { Value = 3, Name = "every 3 hours" },
                new { Value = 4, Name = "every 4 hours" },
                new { Value = 6, Name = "every 6 hours" },
                new { Value = 12, Name = "every 12 hours" },
                new { Value = 24, Name = "each day" },
                new { Value = 168, Name = "each week" }
            };
            return new SelectList(options, "Value", "Name", CacheHours);
        }

        public IEnumerable<SelectListItem> CachePolicyOptions()
        {
            var options = new[]
            {
                new { Value = 2, Name = "each user" },
                new { Value = 1, Name = "all users" }
            };
            return new SelectList(options, "Value", "Name", CachePolicy);
        }

        private void Fill(DashboardWidget widget)
        {
            Name = widget.Name;
            Description = widget.Description;
            HTMLContentId = widget.HTMLContentId ?? 0;
            PythonContentId = widget.PythonContentId ?? 0;
            SQLContentId = widget.SQLContentId ?? 0;
            Enabled = widget.Enabled;
            Order = widget.Order;
            System = widget.System;
            HTMLContent = widget.HTMLContent;
            PythonContent = widget.PythonContent;
            SQLContent = widget.SQLContent;
            CachePolicy = widget.CachePolicy;
            CacheHours = widget.CacheHours;
            CacheExpires = widget.CacheExpires;
            CachedContent = widget.CachedContent;
            Roles = widget.DashboardWidgetRoles.Select(r => r.RoleId).ToArray();
        }

        private void UpdateContent(DashboardWidget widget)
        {
            widget.CopyPropertiesFrom(this, excludefields: "HTMLContentId,PythonContentId,SQLContentId");
            widget.HTMLContent = CurrentDatabase.Contents.Where(c => c.Id == HTMLContentId).SingleOrDefault();
            widget.HTMLContentId = (HTMLContentId == 0) ? (int?)null : HTMLContentId;
            widget.PythonContent = CurrentDatabase.Contents.Where(c => c.Id == PythonContentId).SingleOrDefault();
            widget.PythonContentId = (PythonContentId == 0) ? (int?)null : PythonContentId;
            widget.SQLContent = CurrentDatabase.Contents.Where(c => c.Id == SQLContentId).SingleOrDefault();
            widget.SQLContentId = (SQLContentId == 0) ? (int?)null : SQLContentId;
        }

        public void UpdateModel()
        {
            DashboardWidget widget;
            if (Id != 0)
            {
                // update existing widget
                widget = CurrentDatabase.DashboardWidgets.Where(w => w.Id == Id).Single();
                UpdateContent(widget);
            }
            else
            {
                // create new widget
                widget = new DashboardWidget();
                UpdateContent(widget);
                widget.Order = CurrentDatabase.DashboardWidgets.Max(w => w.Order) + 1;
                CurrentDatabase.DashboardWidgets.InsertOnSubmit(widget);
                CurrentDatabase.SubmitChanges();
                Id = widget.Id;
            }
            SetRoles(Roles);
            SetCachePolicy(widget);
        }

        public static void UpdateWidgetOrder(CMSDataContext db, List<int> widgetIds)
        {
            int on = 1;
            var widgets = db.DashboardWidgets.Where(w => widgetIds.Contains(w.Id)).AsQueryable();
            foreach (int widgetId in widgetIds)
            {
                var widget = widgets.SingleOrDefault(w => w.Id == widgetId);
                if (widget != null)
                {
                    widget.Order = on;
                }
                on++;
            }
            db.SubmitChanges();
        }

        private void SetRoles(int[] roleIds)
        {
            var existing = CurrentDatabase.DashboardWidgetRoles.Where(r => r.WidgetId == Id);
            CurrentDatabase.DashboardWidgetRoles.DeleteAllOnSubmit(existing);
            CurrentDatabase.SubmitChanges();

            if (roleIds != null)
            {
                foreach (int roleId in roleIds)
                {
                    var role = new DashboardWidgetRole { RoleId = roleId, WidgetId = Id };
                    CurrentDatabase.DashboardWidgetRoles.InsertOnSubmit(role);
                }
            }
            CurrentDatabase.SubmitChanges();
        }

        private void SetCachePolicy(DashboardWidget widget)
        {
            // flush the cache whenever a change is made
            widget.CachedContent = null;
            widget.CacheExpires = null;
            CurrentDatabase.SubmitChanges();
        }

        private string CacheForDB()
        {
            var widget = CurrentDatabase.DashboardWidgets.Where(w => w.Id == Id).Single();
            widget.CacheExpires = DateTime.Now.AddHours(CacheHours);
            widget.CachedContent = Generate();
            CurrentDatabase.SubmitChanges();
            return widget.CachedContent;
        }

        private string Generate()
        {
            var m = new PythonScriptModel(CurrentDatabase);
            m.pythonModel.HttpMethod = "get";
            m.pythonModel.DictionaryAdd("HTMLContent", HTMLContent.Body);
            if (SQLContent != null)
            {
                m.pythonModel.DictionaryAdd("SQLContent", SQLContent.Body);
            }
            m.pythonModel.DictionaryAdd("CurrentUser", CurrentDatabase.CurrentUser);
            m.pythonModel.DictionaryAdd("CurrentPerson", CurrentDatabase.CurrentUserPerson);
            m.pythonModel.DictionaryAdd("WidgetId", "widget_" + Id);
            m.pythonModel.DictionaryAdd("WidgetName", Name);
            if (PythonContent == null)
            {
                return m.RunPythonScript("print model.RenderTemplate(Data.HTMLContent)");
            }
            return m.RunPythonScript(PythonContent.Body);
        }

        public string Embed()
        {
            if (CachePolicy == CachePolicies.PerDB.ToInt())
            {
                if (CachedContent.IsNotNull() && DateTime.Now < CacheExpires)
                {
                    return CachedContent;
                } else
                {
                    return CacheForDB();
                }
            }
            return Generate();
        }

        public enum CachePolicies
        {
            NeverCache = 0,
            PerDB = 1,
            PerUser = 2     // cached client side only
        }
    }
}
