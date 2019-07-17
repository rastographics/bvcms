using System.Collections.Generic;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskDocuments : Ask
    {
        public override string Help => @"
Registrants can upload documents with this question.

File extention allows are .doc .docx .jpg .png .pdf .xls .xlsx .csv.

You need to indicate how much documents the registrant needs to upload, the document's name and if it's required.
";
        public bool TargetExtraValue { get; set; }
        public List<OrganizationDocument> list { get; private set; }

        public AskDocuments()
            : base("AskDocuments")
        {
            list = new List<OrganizationDocument>();
        }
        public override void WriteXml(APIWriter w)
        {
            if (list.Count == 0)
                return;
            w.Start(Type)
                .AttrIfTrue("TargetExtraValue", TargetExtraValue);
            foreach (var q in list)
                w.Add("DocumentName", q.DocumentName);
            w.End();
        }
        public new static AskDocuments ReadXml(XElement e)
        {
            var eq = new AskDocuments
            {
                TargetExtraValue = e.Attribute("TargetExtraValue").ToBool(),
            };
            foreach (var ee in e.Elements("DocumentName"))
                eq.list.Add(OrganizationDocument.ReadXml(ee));
            return eq;
        }
        public partial class OrganizationDocument
        {
            public string Name { get; set; }
            public string DocumentName { get; set; }
            public bool Required { get; set; }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static OrganizationDocument ReadXml(XElement e)
            {
                return new OrganizationDocument() { DocumentName = e.Value };
            }
        }
    }
}
