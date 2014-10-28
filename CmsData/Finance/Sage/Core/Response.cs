using System.Linq;
using System.Xml.Linq;

namespace CmsData.Finance.Sage.Core
{
    internal class Response
    {
        protected XElement Data { get; private set; }

        public string Message { get; private set; }

        protected Response(string xmlResponse)
        {
            var xmlDocument = XDocument.Parse(xmlResponse);
            Data = xmlDocument.Descendants("Table1").First();
            Message = Data.Element("MESSAGE").Value.Trim();
        }
    }
}
