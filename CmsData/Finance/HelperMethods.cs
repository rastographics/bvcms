using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Finance
{
    public class HelperMethods
    {
        public static string FormatExpirationDate(int expMonth, int expYear)
        {
            var month = expMonth.ToString();
            if (month.Length == 1 || month.Length == 2)
            {
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
            }
            else
            {
                throw new Exception($"Expiration month incorrect format.");
            }
            var year = expYear.ToString();
            if (year.Length == 4 || year.Length == 2)
            {
                if (year.Length == 4)
                {
                    year = year.Substring(2, 2);
                }
            }
            else
            {
                throw new Exception($"Expiration year incorrect format.");
            }

            var expirationDate = month + year;

            return expirationDate;
        }
    }
}
