using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.Areas.Manage.Models
{
    public class LogonResultModel
    {
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }
}
