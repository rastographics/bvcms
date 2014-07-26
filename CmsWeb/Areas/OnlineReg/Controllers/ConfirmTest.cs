using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Configuration;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CmsWeb.Areas.Manage.Controllers;
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
            var ed = (from i in DbUtil.Db.RegistrationDatas
                where i.Id == id
                select i).SingleOrDefault();
            if (ed == null)
                return Content("no data");
            //var m = Util.DeSerialize<OnlineRegModel>(ed.Data);
            return Content(ed.Data, contentType: "text/xml");
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
        //[Authorize(Roles = "Admin")]
        //public ActionResult ConfirmTest(int? start, int? count)
        //{
        //    IEnumerable<ExtraDatum> q;
        //    q = from ed in DbUtil.Db.ExtraDatas
        //        //where ed.Data.Contains("<OnlineRegModel ")
        //        where ed.Data.Contains("<OnlineRegModel xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"")
        //        orderby ed.Stamp descending
        //        select ed;
        //    var list = q.Skip(start ?? 0).Take(count ?? 200).ToList();
        //    var q2 = new List<ConfirmTestInfo>();
        //    foreach (var ed in list)
        //    {
        //        var xml = ed.Data.Replace(@"<OnlineRegModel xmlns=""http://schemas.datacontract.org/2004/07/CmsWeb.Models"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">", @"<OnlineRegModel>")
        //            .Replace(" i:", " ")
        //            .Replace(@"xmlns:a=""http://schemas.datacontract.org/2004/07/CmsData""", "")
        //            .Replace(@"xmlns:a=""http://schemas.microsoft.com/2003/10/Serialization/Arrays""", "")
        //            .Replace(@"_x003E_k__BackingField", "")
        //            .Replace(@"_x003C_", "")
        //            .Replace(@"<_", "<")
        //            .Replace(@"</_", "</")
        //            .Replace(@" a:", " ")
        //            .Replace(@"<a:", "<")
        //            .Replace(@"</a:", "</")
        //            ;
        //        var x = XDocument.Parse(xml);
        //        var m = new OnlineRegModel();
        //        m.divid = EleVal(x.Root, "Divid").ToInt2();
        //        m.orgid = EleVal(x.Root,"Orgid").ToInt2();
        //        m.masterorgid = EleVal(x.Root,"Masterorgid").ToInt2();
        //        m.username = (EleVal(x.Root, "Username") ?? EleVal(x.Root, "username"));
        //        m.List = new List<OnlineRegPersonModel>();
        //        foreach (var e in x.Descendants("OnlineRegPersonModel"))
        //        {
        //            m.List.Add(new OnlineRegPersonModel
        //            {
        //                PeopleId = e.Element("PeopleId").Value.ToInt2(),
        //                first = e.Element("first").Value,
        //                last = e.Element("last").Value,
        //                orgid = e.Element("orgid").Value.ToInt2(),
        //                dob = e.Element("dob").Value,
        //                phone = e.Element("phone").Value,
        //                email = e.Element("email").Value,
        //                mname = e.Element("mname").Value,
        //                fname = e.Element("fname").Value,
        //                ShowAddress = e.Element("ShowAddress").Value.ToBool(),
        //                address = e.Element("address").Value,
        //                city = e.Element("city").Value,
        //            });
        //        }

        //        try
        //        {
        //            var i = new ConfirmTestInfo
        //            {
        //                ed = ed,
        //                m = m
        //            };
        //            q2.Add(i);
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //    return View(q2);
        //}
    }
}
