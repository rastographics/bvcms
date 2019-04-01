using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.Code
{
    public abstract class CMSWebViewPage<T> : WebViewPage<T>
    {
        public string Setting(string name, string defaultValue = null)
        {
            return DbUtil.Db.Setting(name, defaultValue);
        }

        public bool Setting(string name, bool defaultValue)
        {
            return DbUtil.Db.Setting(name, defaultValue);
        }
    }
}
