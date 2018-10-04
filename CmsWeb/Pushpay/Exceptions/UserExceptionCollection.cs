using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Pushpay.Exceptions
{
    public class UserExceptionCollection : UserException
    {
        public List<UserException> Exceptions = new List<UserException>();
    }
}
