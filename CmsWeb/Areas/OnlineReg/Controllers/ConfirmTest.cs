using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Xml.Linq;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        public class ConfirmTestInfo
        {
            public RegistrationDatum ed;
            public OnlineRegModel m;
        }
        public class TransactionTestInfo
        {
            public RegistrationDatum ed;
            public TransactionInfo ti;
        }
        private string EleVal(XElement r, string name)
        {
            var e = r.Element(name);
            if (e != null)
                return e.Value;
            return null;
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmTest(int? start, int? count)
        {
            IEnumerable<RegistrationDatum> q;
            q = from ed in DbUtil.Db.RegistrationDatas
                orderby ed.Stamp descending
                select ed;
            var list = q.Skip(start ?? 0).Take(count ?? 200).ToList();
            var q2 = new List<ConfirmTestInfo>();
            foreach (var ed in list)
            {
                try
                {
                    var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                    var i = new ConfirmTestInfo
                    {
                        ed = ed,
                        m = m
                    };
                    q2.Add(i);
                }
                catch (Exception)
                {
                }
            }
            return View(q2);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmTestXml(int id)
        {
            var rd = (from i in DbUtil.Db.RegistrationDatas
                where i.Id == id
                select i).SingleOrDefault();
            if (rd == null)
                return Content("no data");
//            var xd = XDocument.Parse(ed.Data);
//            var proc1 = new XProcessingInstruction("xml-stylesheet", @"type=""text/xsl"" href=""/Content/xml.xsl""");
//            var proc2 = new XProcessingInstruction("xml-stylesheet", @"type=""text/css"" href=""/Content/xml.css""");
//            if (xd.Root != null)
//            {
//                xd.Root.AddBeforeSelf(proc1);
//                xd.Root.AddBeforeSelf(proc2);
//                return Content(xd.ToString(), contentType: "text/xml");
//            }
            return Content(rd.Data, contentType: "text/xml");
        }
        [Authorize(Roles = "ManageTransactions,Finance,Admin")]
        public ActionResult RegPeople(int id)
        {
            var q = from i in DbUtil.Db.RegistrationDatas
                    where i.Id == id
                    select i;
            if (!q.Any())
                return Content("no data");
            var q2 = new List<ConfirmTestInfo>();
            foreach (var ed in q)
            {
                try
                {
                    var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
                    var i = new ConfirmTestInfo
                    {
                        ed = ed,
                        m = m
                    };
                    q2.Add(i);
                }
                catch (Exception)
                {
                }
            }
            return View("ConfirmTest", q2);
        }
    }
}
