namespace CmsData.Finance.Acceptiva.Core
{
    /// <summary>
    /// Representation of an ISO3166-1 Country
    /// </summary>
    public class ISO3166Country
    {
        public ISO3166Country(string name, string alpha2, string alpha3, int numericCode)
        {
            this.Name = name;
            this.Alpha2 = alpha2;
            this.Alpha3 = alpha3;
            this.NumericCode = numericCode;
        }

        public string Name { get; private set; }

        public string Alpha2 { get; private set; }

        public string Alpha3 { get; private set; }

        public int NumericCode { get; private set; }
    }
}
