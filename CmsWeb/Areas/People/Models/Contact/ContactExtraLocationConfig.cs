using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class ContactExtraLocationConfig
    {
        private readonly string _xmlContent;
        private IEnumerable<ContactExtraLocation> _locations;

        public ContactExtraLocationConfig(string configContent = null)
        {
            _xmlContent = configContent;
            if (string.IsNullOrWhiteSpace(configContent))
            {
                _xmlContent = DbUtil.Db.ContentOfTypeText("ContactExtraLocationConfig.xml");
            }
        }

        public IEnumerable<ContactExtraLocation> Locations
        {
            get
            {
                if (_locations == null)
                {
                    _locations = GetLocationsFromContent();
                }

                return _locations;
            }
        }

        public string GetLocationFor(int? organizationId, string ministry, string contactType, string contactReason)
        {
            var allMatches = Locations
                .Select(x => new
                {
                    MatchCount = (ValueMatches(x.Ministry, ministry) ? 1 : 0) +
                                 (ValueMatches(x.ContactType, contactType) ? 1 : 0) +
                                 (ValueMatches(x.ContactReason, contactReason) ? 1 : 0),
                    Match = x
                })
                .OrderByDescending(x => x.MatchCount);

            var bestMatch = allMatches
                .Where(x => x.MatchCount == 3)
                .Select(x => x.Match)
                .FirstOrDefault();

            return bestMatch?.ComputeLocationString() ?? StandardLocation(organizationId);
        }

        private static string StandardLocation(int? organizationId)
        {
            return organizationId.HasValue ? "OrganizationStandard" : "PersonStandard";
        }

        private static bool ValueMatches(string left, string right)
        {
            if (string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (string.Compare(left, right.SlugifyString(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (left == null)
            {
                return true;
            }

            return false;
        }

        private IEnumerable<ContactExtraLocation> GetLocationsFromContent()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_xmlContent))
                {
                    return new List<ContactExtraLocation>();
                }

                var xml = XDocument.Parse(_xmlContent);

                var locations = (xml.Element("ContactExtraLocations")?.Elements("Location"))?
                    .Select(location =>
                        new ContactExtraLocation
                        {
                            Ministry = location?.Element("Ministry")?.Attribute("name")?.Value,
                            ContactType = location?.Element("ContactType")?.Attribute("name")?.Value,
                            ContactReason = location?.Element("ContactReason")?.Attribute("name")?.Value
                        });

                return locations?.ToList() ?? new List<ContactExtraLocation>();
            }
            catch (XmlException)
            {
                return new List<ContactExtraLocation>();
            }
        }
    }

    public class ContactExtraLocation
    {
        public string Ministry { get; set; }
        public string ContactType { get; set; }
        public string ContactReason { get; set; }

        public string ComputeLocationString()
        {
            return string.Join("-",
                Ministry.SlugifyString(),
                ContactType.SlugifyString(),
                ContactReason.SlugifyString());
        }
    }
}
