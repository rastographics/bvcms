using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using CmsData;
using CmsWeb.Areas.Public.Models;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class SGMapModel
    {
        public StringBuilder sb = new StringBuilder();

        public SGMapModel(int? id)
        {
            divid = id;
        }

        public SGMapModel(List<Organization> orgList)
        {
            _orgList = orgList;
        }

        private readonly List<Organization> _orgList;
        private readonly int? divid;

        public IEnumerable<SGInfo> SmallGroupInfo()
        {
            var orgTypes = DbUtil.Db.Setting("SGF-OrgTypes", "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));
            var orgIdList = _orgList?.Select(x => x.OrganizationId).Distinct() ?? new List<int>();

            var q = from o in DbUtil.Db.Organizations
                    let host = o.OrganizationMembers.FirstOrDefault(mm => mm.OrgMemMemTags.Any(mt => mt.MemberTag.Name == "HostHome") || mm.PeopleId == o.LeaderId).Person
                    let schedule = o.OrgSchedules.FirstOrDefault().MeetingTime
                    where host != null && (host.PrimaryAddress ?? "") != ""
                    where !divid.HasValue || o.DivOrgs.Any(dd => dd.DivId == divid) || o.DivisionId == divid
                    where !orgIdList.Any() || orgIdList.Contains(o.OrganizationId)
                    where !orgTypes.Any() || orgTypes.Contains(o.OrganizationType.Description)
                    select new
                    {
                        host,
                        o,
                        schedule
                    };

            var q2 = from i in q.ToList()
                     let addr = i.host.AddrCityStateZip
                     join gc in DbUtil.Db.GeoCodes on addr equals gc.Address into g
                     from geocode in g.DefaultIfEmpty()
                     select new SGInfo
                     {
                         desc = i.o.Description ?? i.o.OrganizationName, //o.Description,
                         addr = addr,
                         name = i.o.OrganizationName,
                         schedule = i.schedule,
                         cmshost = DbUtil.Db.CmsHost,
                         id = i.o.OrganizationId,
                         gc = geocode,
                         org = i.o,
                         markertext = i.o.OrganizationExtras.SingleOrDefault(oe => oe.Field == "Term")?.Data == "Beta Group" ? "B" : " ",
                         color = DbUtil.Db.Setting($"UX-MapPinColor-Campus-{i.o.CampusId.GetValueOrDefault(-1)}", "FFFFFF").Replace("#", "")
                     };
            return q2.ToList();
        }

        public IEnumerable<MarkerInfo> Locations()
        {
            var qlist = SmallGroupInfo();

            var addlist = new List<GeoCode>();

            foreach (var i in qlist.Where(ii => ii.gc == null))
            {
                i.gc = addlist.SingleOrDefault(g => g.Address == i.addr)
                    ?? DbUtil.Db.GeoCodes.FirstOrDefault(g => g.Address == i.addr);
                if (i.gc == null)
                {
                    i.gc = GetGeocode(i.addr);
                    addlist.Add(i.gc);
                }
            }
            if (addlist.Count > 0)
                DbUtil.Db.GeoCodes.InsertAllOnSubmit(addlist);
            DbUtil.Db.SubmitChanges();

            var template = DbUtil.Content("SGF-MapTooltip", "");
            if (string.IsNullOrEmpty(template))
            {
                template = @"
<div>
[SGF:Name]<br />
[SGF:Neighborhood]<br />
Group Type: [SGF:Type]<br />
Meeting Time: [SGF:Day] at [SGF:Time]<br />
<a target='smallgroup' href='/OnlineReg/[SGF:OrgID]'>Signup</a>
</div>";
            }

            var loadAllValues = DbUtil.Db.Setting("SGF-LoadAllExtraValues", false);
            var sortSettings = DbUtil.Db.Setting("UX-SGFSortBy", "SGF:Name");

            return (from ql in qlist
                where ql.gc.Latitude != 0
                select new
                {
                    model = ql,
                    dict = GetValuesDictionary(ql.org, loadAllValues)
                })
                .OrderBy(x => x.dict[sortSettings])
                .Select(i => new MarkerInfo
                {
                    html = BuildMapFromTemplate(template, i.dict),
                    org = i.model.org,
                    latitude = i.model.gc.Latitude,
                    longitude = i.model.gc.Longitude,
                    image =
                        $"//chart.googleapis.com/chart?chst=d_map_pin_letter&chld={i.model.markertext}|{i.model.color}"
                });
        }

        public string BuildMapFromTemplate(string template, Dictionary<string, string> values)
        {
            foreach (var pair in values)
            {
                template = template.Replace($"[{pair.Key}]", pair.Value);
            }
 
            return template;
        }

        public Dictionary<string, string> GetValuesDictionary(Organization org, bool loadAllValues)
        {
            var values = new Dictionary<string, string>();

            var leader = (from e in DbUtil.Db.People
                          where e.PeopleId == org.LeaderId
                          select e).SingleOrDefault();

            values["SGF:OrgID"] = org.OrganizationId.ToString();
            values["SGF:Name"] = org.OrganizationName;
            values["SGF:Description"] = org.Description;
            values["SGF:Room"] = org.Location;
            values["SGF:Leader"] = org.LeaderName;
            values["SGF:DateStamp"] = DateTime.Now.ToString("yyyy-MM-dd");
            values["SGF:Schedule"] = "";

            if (leader != null && leader.PictureId != null)
                values["SGF:LeaderPicSrc"] = "/Portrait/" + leader.Picture.SmallId.Value + "?v=" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            else
                values["SGF:LeaderPicSrc"] = "/Portrait/-3";

            if (org.OrgSchedules.Count > 0)
            {
                var count = 0;
                foreach (var schedule in org.OrgSchedules)
                {
                    if (count > 0) values["SGF:Schedule"] += "; ";
                    values["SGF:Schedule"] += GroupLookup.DAY_LAST[schedule.SchedDay ?? 0] + ", " + schedule.SchedTime.ToString2("t"); ;
                    count++;
                }
            }

            foreach (var extra in org.OrganizationExtras)
            {
                if (extra.Field.StartsWith("SGF:"))
                    values[extra.Field] = extra.Data;
                else if (loadAllValues)
                    values[$"SGF:{extra.Field.Replace(" ", "")}"] = extra.Data;
            }

            return values;
        }

        private GeoCode GetGeocode(string address)
        {
            var wc = new WebClient();
            var uaddress = HttpUtility.UrlEncode(address);
            var uri = new Uri($"http://maps.googleapis.com/maps/api/geocode/xml?address={uaddress}&sensor=false");
            var xml = wc.DownloadString(uri);
            var xdoc = XDocument.Parse(xml);
            var status = xdoc.Descendants("status").Single().Value;
            if (status == "ZERO_RESULTS")
                return new GeoCode {Address = address};
            try
            {
                var loc = xdoc.Document.Descendants("location");
                var lat = Convert.ToDouble(loc.Descendants("lat").First().Value);
                var lng = Convert.ToDouble(loc.Descendants("lng").First().Value);
                return new GeoCode
                {
                    Address = address,
                    Latitude = lat,
                    Longitude = lng
                };
            }
            catch (Exception ex)
            {
                sb.AppendLine(address);
                sb.AppendLine(status);
                sb.Append(ex.Message);
                return new GeoCode {Address = address};
            }
        }

        public class SGInfo
        {
            public string desc { get; set; }
            public string addr { get; set; }
            public string name { get; set; }
            public DateTime? schedule { get; set; }
            public string cmshost { get; set; }
            public int id { get; set; }
            public string color { get; set; }
            public string markertext { get; set; }
            public GeoCode gc { get; set; }
            public int campusId { get; set; }
            public Organization org { get; set; }
        }

        public class MarkerInfo
        {
            public string title { get; set; }
            public string html { get; set; }
            public string image { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }

            [ScriptIgnore]
            public Organization org { get; set; }
        }
    }
}
