using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.MobileAPI
{
    public class MobileGivingSummary : Dictionary<string, MobileAnnualGivingSummary>
    {
        public MobileGivingSummary() { } //for deserializing in tests
        public MobileGivingSummary(List<int> years)
        {
            if (years.Count == 0)
            {
                years.Add(DateTime.Now.Year);
            }
            var max = years.Max();
            var min = years.Min();
            for (var y = min; y <= max; y++)
            {
                this[$"{y}"] = new MobileAnnualGivingSummary(y);
            }
        }
    }
}
