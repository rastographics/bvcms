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
        public bool Enabled;
        public int Order;
        public bool System;

        public List<DashboardWidgetRole> DashboardWidgetRoles;

        public Content HTMLContent
        {
            get => CurrentDatabase.Contents.Where(c => c.Id == HTMLContentId).SingleOrDefault();
        }

        public Content PythonContent
        {
            get => CurrentDatabase.Contents.Where(c => c.Id == PythonContentId).SingleOrDefault();
        }

        public Content SQLContent
        {
            get => CurrentDatabase.Contents.Where(c => c.Id == SQLContentId).SingleOrDefault();
        }

        //public List<string> Roles
        //{
        //    get => DashboardWidgetRoles.Select(wr => wr.Role.RoleName).ToList();
        //}

        public CMSDataContext CurrentDatabase { get; set; }
        private Content BlankContent = new Content { Id = 0, Name = "(not specified)" };
        private Role BlankRole = new Role { RoleId = 0, RoleName = "(not specified)" };

        [Obsolete(Errors.ModelBindingConstructorError, false)]
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

        public IEnumerable<SelectListItem> UnassignedRoles()
        {
            IEnumerable<int> AssignedRoleIDs = DashboardWidgetRoles.Select(r => r.RoleId);
            var UnassignedRoles = CurrentDatabase.Roles
                    .Where(r => !AssignedRoleIDs.Contains(r.RoleId))
                    .ToList();
            UnassignedRoles.Add(BlankRole);
            return new SelectList(UnassignedRoles, "RoleId", "RoleName", 0);
        }

        public void Fill(DashboardWidget widget)
        {
            Name = widget.Name;
            Description = widget.Description;
            HTMLContentId = widget.HTMLContentId ?? 0;
            PythonContentId = widget.PythonContentId ?? 0;
            SQLContentId = widget.SQLContentId ?? 0;
            Enabled = widget.Enabled;
            Order = widget.Order;
            System = widget.System;
            DashboardWidgetRoles = widget.DashboardWidgetRoles.ToList();
        }

        public void UpdateModel()
        {
            DashboardWidget widget;
            if (Id != 0)
            {
                // update existing widget
                widget = CurrentDatabase.DashboardWidgets.Where(w => w.Id == Id).Single();
                widget.HTMLContentId = null;
                widget.PythonContentId = null;
                widget.SQLContentId = null;
                widget.CopyPropertiesFrom(this);
            }
            else
            {
                // create new widget
                widget = new DashboardWidget();
                widget.CopyPropertiesFrom(this);
                CurrentDatabase.DashboardWidgets.InsertOnSubmit(widget);
            }
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

        public void SetRoles(List<int> roleIds)
        {
            var existing = CurrentDatabase.DashboardWidgetRoles.Where(r => r.WidgetId == Id);
            CurrentDatabase.DashboardWidgetRoles.DeleteAllOnSubmit(existing);
            CurrentDatabase.SubmitChanges();

            foreach (int roleId in roleIds)
            {
                var role = new DashboardWidgetRole { RoleId = roleId, WidgetId = Id };
                CurrentDatabase.DashboardWidgetRoles.InsertOnSubmit(role);
            }
            CurrentDatabase.SubmitChanges();
        }

        public string Generate()
        {
            // todo: use default python script if none assigned, caching
            var m = new PythonScriptModel(CurrentDatabase);
            m.pythonModel.HttpMethod = "get";
            m.pythonModel.DictionaryAdd("HTMLContent", HTMLContent.Body);
            m.pythonModel.DictionaryAdd("SQLContent", SQLContent.Body);
            m.pythonModel.DictionaryAdd("CurrentUser", CurrentDatabase.CurrentUser);
            m.pythonModel.DictionaryAdd("CurrentPerson", CurrentDatabase.CurrentUserPerson);
            return m.RunPythonScript(PythonContent.Body);
        }
    }
}
