using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData
{
    public enum PaymentProcessTypes
    {
        EmpytProcess = 0,
        OneTimeGiving = 1,
        RecurringGiving = 2,
        OnlineRegistration = 3,
        TemporaryRecurringGiving = 4
    }
}
