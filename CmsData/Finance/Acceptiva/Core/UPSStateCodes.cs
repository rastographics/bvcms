using System.Collections.ObjectModel;
using System.Linq;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class UPSStateCodes
    {
        /// <summary>
        /// Obtain UPS State Codes based on its name adn country.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public static string FromStateCountry(string StateName, string Country, CMSDataContext db)
        {
            if (Country == "United States")
            {
                return db.StateLookups.FirstOrDefault(p => p.StateName == StateName)?.StateCode;
            }
            if (Country == "Canada")
            {
                Collection<CanadaState> collection = BuildCollection();
                return collection.FirstOrDefault(p => p.Name == StateName)?.Code;
            }
            return StateName;
        }

        private static Collection<CanadaState> BuildCollection()
        {
            Collection<CanadaState> collection = new Collection<CanadaState>(){
                new CanadaState("Alberta", "AB"),
                new CanadaState("British Columbia", "BC"),
                new CanadaState("Manitoba", "MB"),
                new CanadaState("New Brunswick", "NB"),
                new CanadaState("Newfoundland and Labrador", "NL"),
                new CanadaState("Northwest Territories", "NT"),
                new CanadaState("Nova Scotia", "NS"),
                new CanadaState("Nunavut", "NU"),
                new CanadaState("Ontario", "ON"),
                new CanadaState("Prince Edward Island", "PE"),
                new CanadaState("Quebec", "QC"),
                new CanadaState("Saskatchewan", "SK"),
                new CanadaState("Yukon", "YT")
            };
            return collection;
        }
    }

    internal class CanadaState
    {
        public CanadaState(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public string Name { get; private set; }

        public string Code { get; private set; }
    }
}
