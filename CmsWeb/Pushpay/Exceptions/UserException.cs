using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Pushpay.Exceptions
{
    public class UserException : Exception
    {
        public UserException() : base() { }
        public UserException(string msg) : base(msg) { }

    }
}
