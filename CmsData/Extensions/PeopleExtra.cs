using System;
using System.Linq;
using UtilityExtensions;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace CmsData
{
    public partial class PeopleExtra
    {
        partial void OnCreated()
        {
            TransactionTime = new DateTime(1900, 1, 1);
        }
    }
}
