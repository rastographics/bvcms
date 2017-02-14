using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using CmsData.API;
using CmsWeb.Controllers;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegModel
    {
        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;

            foreach (var e in x.Root.Elements())
            {
                var name = e.Name.ToString();
                switch (name)
                {
                    case "List":
                        foreach (var ee in e.Elements())
                            _list.Add(Util.DeSerialize<OnlineRegPersonModel>(ee.ToString()));
                        break;
                    case "History":
                        foreach (var ee in e.Elements())
                            _history.Add(ee.Value);
                        break;
                    default:
                        Util.SetPropertyFromText(this, name, e.Value);
                        break;
                }
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            writer.WriteComment(Util.Now.ToString());
            foreach (var pi in typeof(OnlineRegModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                          .Where(vv => vv.CanRead && vv.CanWrite))
            {
                switch (pi.Name)
                {
                    case "List":
                        w.Start("List");
                        foreach (var i in _list)
                            Util.Serialize(i, writer);
                        w.End();
                        break;
                    case "History":
                        w.Start("History");
                        foreach (var i in History)
                            w.Add("item", i);
                        w.End();
                        break;
                    case "password":
                        break;
                    case "testing":
                        if (testing == true)
                            w.Add(pi.Name, testing);
                        break;
                    case "FromMobile":
                        if (FromMobile.HasValue())
                            w.Add(pi.Name, FromMobile);
                        else if (MobileAppMenuController.Source.HasValue())
                            w.Add(pi.Name, MobileAppMenuController.Source);
                        break;
                    case "prospect":
                        if (prospect)
                            w.Add(pi.Name, prospect);
                        break;
                    default:
                        w.Add(pi.Name, pi.GetValue(this, null));
                        break;
                }
            }
        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
    }
}
