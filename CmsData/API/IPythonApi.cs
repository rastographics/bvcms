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

        void EmailContent(object savedQuery, int queuedBy, string fromAddr, string fromName, string subject,
            string contentName);

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

        void UpdateNewMemberClassDateIfNullForLastAttended(object savedQuery, int orgId);

        void AddMembersToOrg(object savedQuery, int orgId);

        void AddMemberToOrg(int pid, int orgId);

        string ExtraValueCode(object pid, string name);

        string ExtraValueText(object pid, string name);

        int ExtraValueInt(object pid, string name);

        DateTime ExtraValueDate(object pid, string name);

        bool ExtraValueBit(object pid, string name);

        APIPerson.Person GetPerson(object pid);
    }
}
