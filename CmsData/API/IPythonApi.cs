using System;
using System.Collections.Generic;

namespace CmsData.API
{
    /// <summary>
    /// List of all API calls documented at http://bvcmsapi.readthedocs.org/en/latest/APIFromPythonScripts.html
    /// </summary>
    public interface IPythonApi
    {
        int DayOfWeek { get; }
        DateTime DateTime { get; }
        bool TestEmail { get; set; }
        bool Transactional { get; set; }

        string CallScript(string scriptname);

        void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string contentName);

        void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string contentName);

        void Email(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject, string body);

        List<int> PeopleIds(object savedQuery);

        List<int> OrganizationIds(int progid, int divid);

        Guid OrgMembersQuery(int progid, int divid, int orgid, string memberTypes);

        void Email2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string body);

        void AddExtraValueCode(object savedQuery, string name, string text);

        void AddExtraValueText(object savedQuery, string name, string text);

        void AddExtraValueDate(object savedQuery, string name, object dt);

        void AddExtraValueInt(object savedQuery, string name, int n);

        void AddExtraValueBool(object savedQuery, string name, bool b);

        void UpdateCampus(object savedQuery, string campus);

        void UpdateMemberStatus(object savedQuery, string status);

        void UpdateNewMemberClassStatus(object savedQuery, string status);

        void UpdateNewMemberClassDate(object savedQuery, object dt);

        void UpdateNewMemberClassDateIfNullForLastAttended(object savedQuery, object orgId);

        void AddMembersToOrg(object savedQuery, object orgId);

        void AddMemberToOrg(object pid, object orgId);

        string ExtraValueCode(object pid, string name);

        string ExtraValueText(object pid, string name);

        int ExtraValueInt(object pid, string name);

        DateTime ExtraValueDate(object pid, string name);

        bool ExtraValueBit(object pid, string name);

        APIPerson.Person GetPerson(object pid);

        void CreateTask(int forPeopleId, Person p, string description);

        void JoinOrg(int orgId, Person p);

        void UpdateField(Person p, string field, object value);

        void EmailReminders(object orgId);

        bool DictionaryIsNotAvailable { get; }

        string Dictionary(string s);

        void DictionaryAdd(string key, string value);

        DateTime DateAddDays(object dt, int days);

        int WeekNumber(object dt);

        DateTime SundayForDate(object dt);

        DateTime SundayForWeek(int year, int week);

        DateTime MostRecentAttendedSunday(int progid);

        int CurrentOrgId { get; set; }

        bool SmtpDebug { set; }

        void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string contentName);

        void EmailContent2(Guid qid, int queuedBy, string fromAddr, string fromName, string subject, string contentName);

        void UpdateNamedField(object savedQuery, string field, object value);

        DateTime ParseDate(string dt);

        string ContentForDate(string contentName, object date);

        string HtmlContent(string name);

        string Replace(string text, string pattern, string replacement);

        void EmailReport(object savedquery, int queuedBy, string fromaddr, string fromname, string subject, string report);

        void EmailReport(string savedquery, int queuedBy, string fromaddr, string fromname, string subject,
            string report, string queryname, string querydescription);

        string FmtPhone(string s, string prefix);

        bool InSubGroup(object pid, object OrgId, string group);

        string Form { get; set; }

        string HttpMethod { get; set; }

        bool InOrg(object pid, object OrgId);

        void AddSubGroup(object pid, object OrgId, string group);

        void RemoveSubGroup(object pid, object OrgId, string group);

        APIOrganization.Organization GetOrganization(object OrgId);
    }
}
