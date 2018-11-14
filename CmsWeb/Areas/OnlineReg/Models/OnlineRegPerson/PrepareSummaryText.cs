using CmsData;
using CmsData.OnlineRegSummaryText;
using System;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {

        public string PrepareSummaryText(CMSDataContext db)
        {
            if (RecordFamilyAttendance())
            {
                return SummarizeFamilyAttendance();
            }

            var om = GetOrgMember();
            if (om == null)
            {
                return "";
            }

            try
            {
                return SummaryInfo.GetResults(DbUtil.Db, om.PeopleId, om.OrganizationId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return "Error producing {details}\n" + ex.Message;
            }
        }

        private string SummarizeFamilyAttendance()
        {
            var sb1 = new StringBuilder();
            sb1.Append("<table>");
            sb1.AppendFormat("<tr><td width='50%'>Org:</td><td width='50%'>{0}</td></tr>\n", org.OrganizationName);
            sb1.AppendFormat("<tr><td>First:</td><td>{0}</td></tr>\n", person.PreferredName);
            sb1.AppendFormat("<tr><td>Last:</td><td>{0}</td></tr>\n", person.LastName);
            var sb = sb1;
            foreach (var m in FamilyAttend.Where(m => m.Attend))
            {
                if (m.PeopleId != null)
                {
                    sb.Append($"<tr><td colspan=\"2\">{m.Name}{(m.Age.HasValue ? $" ({Person.AgeDisplay(m.Age, m.PeopleId)})" : "")}</td></tr>\n");
                }
                else
                {
                    sb.Append($"<tr><td colspan=\"2\">{m.Name}{(m.Age.HasValue ? $" ({Person.AgeDisplay(m.Age, m.PeopleId)})" : "")}");
                    if (m.Email.HasValue())
                    {
                        sb.Append($", {m.Email}");
                    }

                    if (m.Birthday.HasValue())
                    {
                        sb.Append($", {m.Birthday}");
                    }

                    if (m.MaritalId.HasValue)
                    {
                        sb.Append($", {m.Marital}");
                    }

                    if (m.GenderId.HasValue)
                    {
                        sb.Append($", {m.Gender}");
                    }

                    sb.Append("</td></tr>\n");
                }
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
