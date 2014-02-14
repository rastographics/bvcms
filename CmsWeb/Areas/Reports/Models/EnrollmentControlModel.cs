/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using CmsWeb.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections;
using CmsData;
using UtilityExtensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Diagnostics;
using CmsData.Codes;

namespace CmsWeb.Areas.Reports.Models
{
    public class EnrollmentControlModel : OrgSearchModel
    {
        public bool usecurrenttag;

        public class MemberInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Organization { get; set; }
            public string Location { get; set; }
            public string MemberType { get; set; }
        }
        public IEnumerable<MemberInfo> list()
        {
            var orgs = FetchOrgs();
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
    }
}

