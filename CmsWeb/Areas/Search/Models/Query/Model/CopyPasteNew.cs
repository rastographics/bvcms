using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public partial class QueryModel
    {
        [Serializable]
        public class ClipboardItem
        {
            public string from { get; set; }
            public Guid guid { get; set; }
            public string xml { get; set; }

            public ClipboardItem(string @from, Guid guid, string xml)
            {
                this.@from = @from;
                this.guid = guid;
                this.xml = xml;
            }
        }

        public void Paste(Guid id)
        {
            var clip = HttpContext.Current.Session["QueryClipboard"] as ClipboardItem;
            if (clip == null)
                return;
            var newclause = Condition.Import(clip.xml, newGuids: clip.from == "copy");
            Condition prevParent = null;
            if (clip.from == "cut")
                if (clip.guid != TopClause.Id)
                {
                    var originalquery = Db.LoadQueryById2(clip.guid).ToClause();
                    var origclause = originalquery.AllConditions[newclause.Id];
                    originalquery.AllConditions.Remove(newclause.Id);
                    if (!origclause.Parent.Conditions.Any())
                        originalquery.AllConditions.Remove(origclause.Parent.Id);
                    originalquery.Save(Db);
                }
                else
                    prevParent = TopClause.AllConditions[newclause.Id].Parent;

            newclause.AllConditions = Current.AllConditions;
            Current.AllConditions[newclause.Id] = newclause;


            if (Current.IsGroup)
            {
                newclause.Order = Current.MaxClauseOrder() + 2;
                newclause.ParentId = Current.Id;
            }
            else
            {
                newclause.Order = Current.Order + 1;
                newclause.ParentId = Current.Parent.Id;
                Current.Parent.ReorderClauses();
            }
            if (prevParent != null && !prevParent.Conditions.Any())
                TopClause.AllConditions.Remove(prevParent.Id);
            TopClause.Save(Db, increment: true);
        }
        public Guid AddConditionToGroup()
        {
            var nc = Current.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Guid AddGroupToGroup()
        {
            var g = Current.AddNewGroupClause();
            var nc = g.AddNewClause();
            TopClause.Save(Db);
            return nc.Id;
        }
        public Condition Current
        {
            get
            {
                var gid = SelectedId ?? Guid.Empty;
                return TopClause.AllConditions[gid];
            }
        }
        public void DeleteCondition()
        {
            Current.DeleteClause();
            TopClause.Save(Db, increment: true);
        }
        public void InsertGroupAbove()
        {
            var g = new Condition
            {
                Id = Guid.NewGuid(),
                ConditionName = QueryType.Group.ToString(),
                Comparison = CompareType.AnyTrue.ToString(),
                AllConditions = Current.AllConditions
            };
            if (Current.IsFirst)
            {
                Current.ParentId = g.Id;
                g.ParentId = null;
            }
            else
            {
                var list = Current.Parent.Conditions.Where(cc => cc.Order >= Current.Order).ToList();
                g.ParentId = Current.ParentId;
                foreach (var c in list)
                    c.ParentId = g.Id;
                g.Order = Current.MaxClauseOrder();
            }
            Current.AllConditions.Add(g.Id, g);
            if (g.IsFirst)
            {
                TopClause = g;
                SelectedId = g.Id;
            }
            TopClause.Save(Db, increment: true);
        }
    }
}