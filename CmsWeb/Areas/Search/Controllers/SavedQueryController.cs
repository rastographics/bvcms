using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;
using Dapper;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "SavedQuery"), Route("{action}/{id?}")]
    public class SavedQueryController : CmsStaffController
    {
        [HttpGet, Route("~/SavedQueryList")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel
            {
                OnlyMine = DbUtil.Db.UserPreference("SavedQueryOnlyMine", "false").ToBool()
            };
            //m.Pager.Set("/SavedQuery/Results", 1, null, "Last Run", "desc");
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SavedQueryModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id);
            if (m.Name.Equals(Util.ScratchPad2))
                m.Name = "copy of scratchpad";
            return View(m);
        }
        [HttpPost]
        public ActionResult Update(SavedQueryInfo m)
        {
            if (m.Name.Equal(Util.ScratchPad2))
                m.Ispublic = false;
            m.CanDelete = true; // must be true since they can edit if they got here
            m.UpdateModel();
            return View("Row", m);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            DbUtil.Db.Queries.DeleteOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }

        internal const string SqlSavedqueries = @"
SELECT 
	QueryId,
    owner ,
    name,
    text
FROM dbo.Query q
join QueryAnalysis qa ON qa.Id = q.QueryId
--WHERE OriginalCount <> ParsedCount
/*
SELECT 
	QueryId,
    owner ,
    name,
    text
FROM dbo.Query q
WHERE name IS NOT NULL
AND name <> 'scratchpad'
AND text not like '%AnyFalse%'
ORDER BY lastRun DESC
*/
";
        [HttpGet]
        public ActionResult UpdateAll()
        {
            var db = DbUtil.Db;
            var list = db.Connection.Query(SqlSavedqueries).ToList();
            foreach (var sq in list)
            {
                var g = sq.QueryId as Guid?;
                if (!g.HasValue)
                    continue;
                DbUtil.DbDispose();
                var c = DbUtil.Db.LoadExistingQuery(g.Value);
                var s = c.ToCode();
                if (s.HasValue())
                    UpdateQueryConditions.Run(sq.QueryId);
            }
            return RedirectToAction("Code");
        }
        [HttpGet]
        public ActionResult Code()
        {
            return View(new CodeModel(CodeQueries));
        }
        [HttpGet]
        public ActionResult CodeAnalysis()
        {
            DoAnalysis();
            return Redirect("Code");
        }
        [HttpGet]
        public ActionResult CodeAnalysisAll()
        {
            var list = new List<string>
            {
                "2pc", "abbashouse", "abchurch", "abundantlife", "accc", "alaog", "alliancechurch", "alphaumc", "ambassadorchurch", "andrew", "applewoodbaptist", "arapahoroad", "arisememphis", "asischurch", "auburncornerstone", "austinstreetbc", "baptisttabernacle", "bbckpt",
                "bellevue", "bethany", "bethanylcw", "bethel-clydach", "bethelbaptistbrookings", "bethelnp", "bethelsi", "bgv", "biblebapt", "biblesforisrael", "bigtimberchurch", "bjcopp", "blendvillechristian", "bridgememphis", "bryan", "calvarychapelbaptist", "calvarychapelstuart", "calvarychurchvg", "calvarytriad", "camcc", "canopyroads", "cantonbaptisttemple", "capitalwestcc", "cbcfolsom", "cbcspringhill", "cccnc", "cccnyc", "cfindy", "chapelhilllife", "cherokeebaptist", "christembassyarlington", "christembassycypress", "christiansburgbaptist", "christlifecc", "christlutheranvail", "churchatriverhills", "churchrox", "citadelchurch", "citycenterchurch", "cloughpike", "corinthbaptist", "cornerstoneag", "cornerstonecc", "cornerstonejeffcity", "cottonwood", "covenantag", "covenantlifetampa", "covpres", "cross-point", "crosspointdublin", "crosspointetampa", "crossroadoc", "crossroads4me", "crossroadsofjoliet", "cspc", "deervalleychurch", "discovercrosspoint", "doorbrekers", "eastbrent", "eastpointdesoto", "ebccollierville", "ebcfamily", "ebcjackson", "emmauslutheran", "enonbc", "epic4life", "essentialchurch", "explorecrossway", "faith", "faith4duncan", "faithchristians", "faithkalispell", "familybible", "fayettebaptist", "fbc-la", "fbc-online", "fbcalbany", "fbcdecom", "fbcelkton", "fbcfayetteville", "fbcgalax", "fbcgoodlettsville", "fbchowe", "fbchw", "fbcmableton", "fbcrockport", "fbcscott", "fcbc", "fcbcwalnut", "fccdc", "fcchudson", "fcclw", "fccsaugatuck", "fccwarsaw", "fecc", "fellowshipoffaith", "findnewlife", "firehousecommunityoutreach", "firstaztec", "firstevan", "firstfwb", "flagchurch", "foundrychurch", "freshwaterchurchstjohn", "friendship-church", "friendshipbaptist411", "friendshipcommunitychurch", "frpcs", "fusionchurchofmadison", "fwcdc", "gatewaysavannah", "gbcdillon", "geistchapel", "genesischurchcma", "gladtidings", "goldsbybaptistchurch", "goredemption", "gothenburgefc", "Gracebaptistofkettering", "gracecc", "graceefc", "graceindallas", "gracelouisville", "gracepointe", "gracepointebc", "granitesprings", "greenvillefcc", "gsbcfamily", "gtofgibson", "hangingmoss", "harvestknoxville", "harvestmemphis", "harvestworld", "haywardwesleyan", "hbchaslet", "hbcministry", "hbctexas", "hcaz", "hearlistenobey", "heartchurch", "hgbc", "highpointcommunitychurch", "hillcrestbaptistchurch", "hillsborofirstumc", "hillschurchoc", "historicebenezer", "hollycreekbaptist", "holytrinityanglicanchurch", "hope", "hope-umc", "hopecommunity", "hopeelca", "hunterstreet", "ibcky", "ibcspringfield", "impactkc", "inglewoodbaptist", "jcboston", "journeyalaska", "journeydepere", "karen2", "kirkplace", "Kyle", "lakehomacoc", "lakeviewchurch", "lbcf", "lbctheodore",
                "leavener", "legacychurchcolumbus", "lhbc", "lifechurchsouthfield", "lifenowchurch", "lifepoint365", "lifepointbc", "lifetransformation", "lighthousebapt", "livinglifechurch", "livingwaterlutheran", "lmbcorg", "longviewpoint", "loveservereach", "maconroadbaptist", "meadowpark", "medfordumc", "mediaministers", "metbiblechurch", "metrocalvary", "midwayfirstbaptist", "midwaywestchurch", "missionbaptist", "mohundro", "mpchurchpa", "msbcmemphis", "mtrosemedia", "mvcglenwood", "Mychurchspringfield", "mycsbc", "mygrace", "mypcbc", "mzlife", "nebaptist", "newlifethefort", "newlifewv", "newpromisechurch", "newsalem", "nhccgilroy", "nlcchurch", "north-campus", "northcincy", "northeastcc", "northeastchristian", "northpointebc", "ntbc", "nwbbc", "nwhills", "oakbrookchurch", "oaklandbaptist", "orchardfellowship", "otayranchbaptistchurch", "pacificcrossroads", "pagosabiblechurch", "parksidebaptist", "pbcbartlett", "pibaustin",
                "plattsburghnazarene", "pointharbor", "porticobtown", "pray", "providencehill", "purposecentre", "questcebu", "raleighassembly", "rbchelena", "redeemer", "refuge-memphis", "revivemylife", "Rh-church", "rhbchurch", "ridgechurch", "ridgelinecc", "ridgepointchurch", "riverrockchurch", "riversideconnect", "rockpointechurch", "salivingfaith", "sbcozark", "seattlecbc", "secondunitarian", "sfcsa", "shawneepark", "shekinahatl", "shofaronline", "silverdale", "silverspringumcp", "sixthstreetbaptistchurch", "sjcc", "sjvan", "slavelake-stpetersecumenical", "smc", "smihaiti", "spcelina", "spiritfilledchurch", "sportsmenchurch-stevensville", "spotswood",
                "stanwichchurch", "stathamchurch", "stdavidsdenton", "stillwaterumc", "stjohnsvancouver", "stlukesmarianna", "stonebridgefwb", "storychurch", "stpatrickpres", "stpetersaz", "stpetersfireside", "stuartheights", "suburbanchurch", "summitchurchvt", "syarbrough", "tallahasseebikerchurch", "tbctrinity", "tcal", "tcg", "teaysvalleybaptist", "theanchorbiblechurch", "thebeaconoc", "Thebridgewired", "thecentralbaptist", "thecrossroadscog", "thefusionchurch", "thegrovelife", "theheightsonline", "thejourneyumc", "theparkschurch", "theparkschurchmelissa", "therocc", "thesanctuarymilton", "thetabledc", "thewellvegas", "timberlakebaptist", "topekafirst", "treg", "trinitycrs", "tumct", "valleybible", "valleymadison", "vbcbackgate", "vbclex", "vbcmilton", "verdebaptist", "Victorybbc", "visionbaptist", "westbaptist", "westfloridabaptist", "whbconline", "will", "wjbc", "woodlandhillstallahassee", "woodlandsadventfellowship", "woodridgebc",
            };
            var sb = new StringBuilder();
            foreach (var db in list)
                sb.Append(DoAnalysis(db));
            return Content(sb.ToString(), "text/plain");
        }
        private string DoAnalysis(string host = null)
        {
            if (host.HasValue())
            {
                HttpRuntime.Cache["testhost"] = host;
                DbUtil.DbDispose();
            }
            var sb = new StringBuilder();
            var m = new CodeModel(AnalizeQueries);
            foreach(var q in m.List)
                sb.Append(m.Analyze(q));
            return sb.ToString();
        }

            const string AnalizeQueries = @"
SELECT 
	q.QueryId ,
    q.text ,
    q.owner ,
    q.created ,
    q.lastRun ,
    q.name ,
    q.ispublic ,
    qa.Seconds ,
    qa.OriginalCount ,
    qa.ParsedCount ,
    qa.Message
FROM dbo.Query q
left join QueryAnalysis qa ON qa.Id = q.QueryId
	WHERE (qa.OriginalCount <> qa.ParsedCount AND qa.OriginalCount IS NOT NULL)
	OR (Message LIKE '%xml version%')
    OR (Message NOT LIKE '%xml version%')
--WHERE name IS NOT NULL
--AND name <> 'scratchpad'
--AND text not like '%AnyFalse%'
--AND text NOT LIKE '%ContributionAmount2%'
ORDER BY q.lastRun desc
";
            const string CodeQueries = @"
SELECT 
	q.QueryId ,
    q.text ,
    q.owner ,
    q.created ,
    q.lastRun ,
    q.name ,
    q.ispublic ,
    qa.Seconds ,
    qa.OriginalCount ,
    qa.ParsedCount ,
    qa.Message
FROM dbo.Query q
left join QueryAnalysis qa ON qa.Id = q.QueryId
	WHERE (qa.OriginalCount <> qa.ParsedCount AND qa.OriginalCount IS NOT NULL)
	OR (Message LIKE '%xml version%')
    OR (Message NOT LIKE '%xml version%')
--WHERE name IS NOT NULL
--AND name <> 'scratchpad'
--AND text not like '%AnyFalse%'
--AND text NOT LIKE '%ContributionAmount2%'
ORDER BY q.lastRun desc
";
        public class CodeModel
        {
            public List<dynamic> List;
            public int Count;
            public string Code;

            public CodeModel(string queries)
            {
                var populateSql = @"
INSERT dbo.QueryAnalysis ( Id )
SELECT q.QueryId 
FROM dbo.Query q
left join QueryAnalysis qa ON qa.Id = q.QueryId
--WHERE OriginalCount <> ParsedCount
WHERE name IS NOT NULL
AND name <> 'scratchpad'
AND text not like '%AnyFalse%'
AND text NOT LIKE '%ContributionAmount2%'
";
                if (!DbUtil.Db.QueryAnalyses.Any())
                    DbUtil.Db.ExecuteCommand(populateSql);
                List = DbUtil.Db.Connection.Query(queries).ToList();
                Count = List.Count;
                Debug.WriteLine($"{Util.Host} Count: {Count}");
            }

            public Guid? Existing;
            public Guid? Parsed;
            public string Error;
            public string Message;
            public string Xml;

            public void GetLinks(dynamic q)
            {
                Existing = q.QueryId as Guid?;
                Xml = q.text as string;
                Parsed = null;
                Error = null;
                if (Existing == null)
                    return;
                var c = DbUtil.Db.LoadExistingQuery(Existing.Value);
                Code = c.ToCode();
            }
            public string Analyze(dynamic q)
            {
                Count--;
                Existing = q.QueryId as Guid?;
                var id = Existing;
                Xml = q.text as string;
                Parsed = null;
                Error = null;
                if (Existing == null)
                    return null;
                var cnt1 = 0;
                var cnt2 = 0;
                var c = DbUtil.Db.LoadExistingQuery(Existing.Value);
                Code = c.ToCode();
                double? seconds = null;
                var sb = new StringBuilder();
                try
                {
                    var dt = DateTime.Now;
                    cnt1 = DbUtil.Db.PeopleQueryCondition(c).Count();
                    seconds = DateTime.Now.Subtract(dt).TotalSeconds;
                    Message = $"Count={Count} {seconds:N0} {Existing}";
                    var s = $"{Util.Host} {Existing} {Count}  {seconds} seconds";
                    sb.AppendLine(s);
                    Debug.WriteLine(s);

                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }

                if (!Code.HasValue())
                    return sb.ToString();
                try
                {
                    cnt2 = DbUtil.Db.PeopleQueryCode(Code).Count();
                    if (cnt2 != cnt1)
                    {
                        Error = $"Original={cnt1:N0}  Parsed={cnt2:N0}";
                        var s = $"{Util.Host} {Existing} {Count}  Original={cnt1:N0}  Parsed={cnt2:N0}";
                        sb.AppendLine(s);
                        Debug.WriteLine(s);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    var s = $"{Util.Host} {Existing} {Count}  {Error}";
                    sb.AppendLine(s);
                    Debug.WriteLine(s);
                }
                DbUtil.Db.Connection.Execute(@"
UPDATE QueryAnalysis 
set seconds = @seconds, 
    Message = @Error, 
    OriginalCount = @cnt1, 
    ParsedCount=@cnt2 
where Id = @id", new { id, seconds, Error, cnt1, cnt2 });
                return sb.ToString();
            }
        }
    }
}
