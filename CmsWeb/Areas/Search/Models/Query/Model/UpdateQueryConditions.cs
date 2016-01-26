using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class UpdateQueryConditions
    {
        private QueryModel m;
        private int? userpeopleid;

        public static void Run(Guid id)
        {
            var mm = new UpdateQueryConditions();
            try
            {
                mm.UpdateConditions(id);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        private void UpdateConditions(Guid id)
        {
            var query = DbUtil.Db.LoadQueryById2(id);
            userpeopleid = (from u in DbUtil.Db.Users
                            where u.Username == query.Owner
                            select u.PeopleId).SingleOrDefault();
            var c = Condition.Import(query.Text, query.Name, newGuids: false, topguid: id);
            m = new QueryModel { TopClause = c };
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, settings))
                UpdateCondition(w, id);
            query.Text = sb.ToString();
            DbUtil.Db.SubmitChanges();
        }
        private void UpdateCondition(XmlWriter w, Guid id)
        {
            m.SelectedId = id;
            m.EditCondition();
            w.WriteStartElement("Condition");
            UpdateAttributes(w);
            foreach (var qc in m.Selected.Conditions)
                UpdateCondition(w, qc.Id);
            w.WriteEndElement();
        }
        private void UpdateAttributes(XmlWriter w)
        {
            UpdateIdValues();

            w.WriteAttr("Id", m.Selected.Id.ToString());
            w.WriteAttr("Order", m.Selected.Order.ToString());
            w.WriteAttr("Field", m.ConditionName);
            w.WriteAttr("Comparison", m.Comparison, "AllTrue");
            w.WriteAttr("Description", m.Description, "scratchpad");
            w.WriteAttr("PreviousName", m.Description, "scratchpad");
            w.WriteAttr("TextValue", m.TextValue);
            w.WriteAttr("DateValue", m.DateValue);
            w.WriteAttr("CodeIdValue", m.CodeIdValue);
            w.WriteAttr("StartDate", m.StartDate);
            w.WriteAttr("EndDate", m.EndDate);
            w.WriteAttr("Program", m.Program);
            w.WriteAttr("Division", m.Division);
            w.WriteAttr("Organization", m.Organization);
            w.WriteAttr("OrgType", m.OrgType);
            w.WriteAttr("Days", m.Days);
            w.WriteAttr("Quarters", m.Quarters);
            w.WriteAttr("Tags", m.Tags);
            w.WriteAttr("Schedule", m.Schedule);
            w.WriteAttr("Campus", m.Campus);
            if (m.ConditionName == "FamilyHasChildrenAged")
                w.WriteAttr("Age", m.Age ?? 0, 12);
            w.WriteAttr("SavedQueryIdDesc", m.SavedQuery, "scratchpad");
            w.WriteAttr("OnlineReg", m.OnlineReg);
            w.WriteAttr("OrgStatus", m.OrgStatus);
            w.WriteAttr("OrgType2", m.OrgType2);
            w.WriteAttr("OrgName", m.OrgName);
        }

        private void UpdateIdValues()
        {
            if (m.Selected.IsGroup)
                return;

            IEnumerable<string> q = null;

            var d = m.GetCodeData();
            if (d != null)
            {
                q = from v in d
                    where v.Selected
                    select v.Value;
                m.CodeIdValue = string.Join(";", q);
            }
            if(m.ProgramVisible && m.Program.HasValue())
            {
                q = from v in m.Programs()
                    where v.Selected
                    select v.Value;
                m.Program = q.SingleOrDefault();
            }
            if(m.DivisionVisible && m.Division.HasValue())
            {
                q = from v in m.Divisions(m.Program)
                    where v.Selected
                    select v.Value;
                m.Division = q.SingleOrDefault();
            }
            if(m.OrganizationVisible && m.Organization.HasValue())
            {
                q = from v in m.Organizations(m.Division)
                    where v.Selected
                    select v.Value;
                m.Organization = q.SingleOrDefault();
            }
            if(m.OrgTypeVisible && m.OrgType.HasValue())
            {
                q = from v in m.OrgTypes()
                    where v.Selected
                    select v.Value;
                m.OrgType = q.SingleOrDefault();
            }
            if (userpeopleid.HasValue && m.TagsVisible && m.Tags.HasValue())
            {
                q = from v in m.TagData(userpeopleid)
                    where v.Selected
                    select v.Value;
                m.Tags = string.Join(";", q);
            }
            if(m.ScheduleVisible && m.Schedule.HasValue())
            {
                q = from v in m.Schedules()
                    where v.Selected
                    select v.Value;
                m.Schedule = q.SingleOrDefault();
            }
            if(m.CampusVisible && m.Campus.HasValue())
            {
                q = from v in m.Campuses()
                    where v.Selected
                    select v.Value;
                m.Campus = q.SingleOrDefault();
            }
            if(m.OnlineRegVisible && m.OnlineReg.HasValue())
            {
                q = from v in m.RegistrationTypeIds()
                    where v.Selected
                    select v.Value;
                m.OnlineReg = q.SingleOrDefault();
            }
            if(m.OrgStatusVisible)
            {
                q = from v in m.StatusIds()
                    where v.Selected
                    select v.Value;
                m.OrgStatus = q.SingleOrDefault() ?? "";
            }
            if(m.SavedQueryVisible)
            {
                q = from v in m.SavedQueries()
                    where v.Selected
                    select v.Value;
                m.SavedQuery = q.SingleOrDefault();
            }
        }
    }
}