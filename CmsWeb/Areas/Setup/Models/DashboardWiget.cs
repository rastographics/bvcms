using CmsData;
using CmsData.Codes;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Models
{
    public class DashboardWidgetModel
    {
        public int Id;

        public DashboardWidget Widget;

        private CMSDataContext CurrentDatabase;

        public DashboardWidgetModel(string id, CMSDataContext db) {
            CurrentDatabase = db;
            if (id.ToLower() == "new")
            {
                Widget = new DashboardWidget();
                Id = 0;
            }
            else
            {
                Id = id.ToInt();
                Widget = CurrentDatabase.DashboardWidgets.Where(w => w.Id == id.ToInt()).Single();
            }
        }
        
        public IEnumerable<Content> HtmlContents()
        {
            return CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypeHtml)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
        }

        public IEnumerable<Content> SQLContents()
        {
            return CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypeSqlScript)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
        }

        public IEnumerable<Content> PythonContents()
        {
            return CurrentDatabase.Contents
                    .Where(c => c.TypeID == ContentTypeCode.TypePythonScript)
                    .Where(c => c.ContentKeyWords.Any(vv => vv.Word == "widget"))
                    .ToList();
        }

        public IEnumerable<Role> UnassignedRoles()
        {
            IEnumerable<int> AssignedRoleIDs = Widget.DashboardWidgetRoles.Select(r => r.RoleId);
            return CurrentDatabase.Roles
                    .Where(r => !AssignedRoleIDs.Contains(r.RoleId))
                    .ToList();
        }
    }
}
