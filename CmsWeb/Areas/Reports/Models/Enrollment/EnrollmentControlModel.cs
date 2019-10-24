/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */

using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Reports.Models
{
    public class EnrollmentControlModel
    {        
        public class MemberInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public string MemberType { get; set; }
        }
        public EnrollmentControlModel() { }
        public static IEnumerable<MemberInfo> List(OrgSearchModel model, string na = "", bool usecurrenttag = false)
        {
            var orgs = model.FetchOrgs();
            var q = from m in DbUtil.Db.OrganizationMembers
                    join o in orgs on m.OrganizationId equals o.OrganizationId
                    select m;
            if (usecurrenttag)
            {
                var tagid = DbUtil.Db.TagCurrent().Id;
                q = from m in q
                    where m.Person.Tags.Any(tt => tt.Id == tagid)
                    select m;
            }
            var q2 = from m in q
                     where m.Person.LastName.StartsWith(na)
                     orderby m.Person.Name2
                     select new MemberInfo
                     {
                         Name = m.Person.Name2,
                         Id = m.PeopleId,
                         Organization = m.Organization.OrganizationName,
                         Location = m.Organization.Location,
                         MemberType = m.MemberType.Description,
                     };
            return q2;
        }

        public static List<string> LastNameStarts(OrgSearchModel m)
        {
            var cn = DbUtil.Db.Connection;
            var q2 = cn.Query<string>(@"
SELECT na FROM (
	SELECT SUBSTRING(LastName, 1, 2) na FROM dbo.People p
	JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId
	JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
    JOIN dbo.PeopleIdsFromOrgSearch(@name, @prog, @div, @type, @campus, @sched, @status, @onlinereg, @mainfellowship, @parentorg) pids ON pids.PeopleId = om.PeopleId
	WHERE EXISTS(SELECT NULL 
				FROM dbo.DivOrg dd
				JOIN dbo.ProgDiv pp ON pp.DivId = dd.DivId
				WHERE OrgId = o.OrganizationId)
) tt
GROUP BY na
ORDER BY na
", new
            {
                name = m.Name,
                prog = m.ProgramId,
                div = m.DivisionId,
                type = m.TypeId,
                campus = m.CampusId,
                sched = m.ScheduleId,
                status = m.StatusId,
                onlinereg = m.OnlineReg,
                mainfellowship = m.TypeId == CodeValueModel.OrgType.MainFellowship,
                parentorg = m.TypeId == CodeValueModel.OrgType.ParentOrg
            });
            return q2.ToList();
        }
    }
}

