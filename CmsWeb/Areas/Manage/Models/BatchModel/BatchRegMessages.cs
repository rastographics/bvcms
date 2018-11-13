using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using System.Linq;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public class BatchRegMessages
    {
        private Settings os;
        public static void Update(string text)
        {
            new BatchRegMessages().DoUpdate(text);
        }
        private void DoUpdate(string text)
        {
            var xdoc = XDocument.Parse(text.TrimStart());
            if (xdoc.Root == null)
            {
                throw new UserInputException("could not parse xml document");
            }

            foreach (var x in xdoc.Root.Elements("Messages"))
            {
                var oid = x.Attribute("id").Value.ToInt();
                os = DbUtil.Db.CreateRegistrationSettings(oid);
                var o = DbUtil.Db.LoadOrganizationById(oid);
                foreach (var e in x.Elements())
                {
                    var name = e.Name.ToString();
                    switch (name)
                    {
                        case "Confirmation":
                            SubjectAndBody("", e); // no prefix
                            break;
                        case "Reminder":
                            SubjectAndBody("Reminder", e);
                            break;
                        case "SupportEmail":
                            SubjectAndBody("Support", e);
                            break;
                        case "SenderEmail":
                            SubjectAndBody("Sender", e);
                            break;
                        case "Instructions":
                            Instruction(e);
                            break;
                    }
                }
                o.UpdateRegSetting(os);
                DbUtil.Db.SubmitChanges();
            }
        }

        private void SubjectAndBody(string prefix, XContainer e)
        {
            foreach (var ee in e.Elements())
            {
                var nname = ee.Name.ToString();
                switch (nname)
                {
                    case "Subject":
                        Util.SetProperty(os, prefix + "Subject", ee.Value);
                        break;
                    case "Body":
                        Util.SetProperty(os, prefix + "Body", ee.Value);
                        break;
                }
            }
        }
        private void Instruction(XContainer e)
        {
            foreach (var ee in e.Elements())
            {
                var instrutions = new[] { "Login", "Select", "Find", "Options", "Special", "Submit", "Sorry", };
                var nname = ee.Name.ToString();
                switch (nname)
                {
                    case "ThankYouMessage":
                        os.ThankYouMessage = ee.Value;
                        break;
                    case "Terms":
                        os.Terms = ee.Value;
                        break;
                    default:
                        if (instrutions.Contains(nname))
                        {
                            Util.SetProperty(os, "Instruction" + nname, ee.Value);
                        }

                        break;
                }
            }
        }
    }
}
