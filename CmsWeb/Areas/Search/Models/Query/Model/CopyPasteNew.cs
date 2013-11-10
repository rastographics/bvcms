using System;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        public static bool ClipboardHasCondition()
        {
            var clip = HttpContext.Current.Session["QueryClipboard"] as string;
            return clip != null;
        }

        public void Copy()
        {
            HttpContext.Current.Session["QueryClipboard"] = Selected.ToXml(newGuids: true);
        }
        public void Cut()
        {
            HttpContext.Current.Session["QueryClipboard"] = Selected.ToXml();
            Selected.DeleteClause();
            TopClause.Save(Db, increment: true);
        }

        public void Paste()
        {
            var clip = HttpContext.Current.Session["QueryClipboard"] as string;
            if (clip == null)
                return;
            var newclause = Condition.Import(clip);

            foreach(var c in newclause.AllConditions)
                Selected.AllConditions[c.Key] = c.Value;;
            newclause.AllConditions = Selected.AllConditions;

            if (Selected.IsGroup)
            {
                newclause.Order = Selected.MaxClauseOrder() + 2;
                newclause.ParentId = Selected.Id;
            }
            else
            {
                newclause.Order = Selected.Order + 1;
                newclause.ParentId = Selected.Parent.Id;
                Selected.Parent.ReorderClauses();
            }
            TopClause.Save(Db, increment: true);
        }
        public Guid AddConditionToGroup()
        {
            var nc = Selected.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Guid AddGroupToGroup()
        {
            var g = Selected.AddNewGroupClause();
            var nc = g.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Condition Selected
        {
            get
            {
                var gid = SelectedId ?? Guid.Empty;
                return TopClause.AllConditions[gid];
            }
        }
        public void DeleteCondition()
        {
            Selected.DeleteClause();
            TopClause.Save(Db, increment: true);
        }
        public void InsertGroupAbove()
        {
            var g = new Condition
            {
                Id = Guid.NewGuid(),
                ConditionName = QueryType.Group.ToString(),
                Comparison = CompareType.AnyTrue.ToString(),
                AllConditions = Selected.AllConditions
            };
            if (Selected.IsFirst)
            {
                Selected.ParentId = g.Id;
                g.ParentId = null;
            }
            else
            {
                var list = Selected.Parent.Conditions.Where(cc => cc.Order >= Selected.Order).ToList();
                g.ParentId = Selected.ParentId;
                foreach (var c in list)
                    c.ParentId = g.Id;
                g.Order = Selected.MaxClauseOrder();
            }
            Selected.AllConditions.Add(g.Id, g);
            if (g.IsFirst)
            {
                TopClause = g;
                SelectedId = g.Id;
            }
            TopClause.Save(Db, increment: true);
        }
    }
}