using CmsData;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class GoogleRecaptcha
    {
        public static bool IsValidResponse(HttpContextBase httpContext, CMSDataContext db)
        {
            //var db = DbUtil.Create(httpContext.Request.Url.Authority.SplitStr(".:")[0]);
            var secret = db.Setting("GoogleReCaptchaSecretKey", ConfigurationManager.AppSettings["GoogleReCaptchaSecretKey"]);
            var siteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
            var remoteIpAddress = httpContext.Request.UserHostAddress;
            var valid = false;
            var responseData = httpContext.Request["g-recaptcha-response"];
            if (secret.HasValue() && responseData.HasValue())
            {
                try
                {
                    using (var webclient = new WebClient())
                    {
                        webclient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                        byte[] result = webclient.UploadValues(siteVerifyUrl, null, new RecaptchaVerification
                        {
                            secret = secret,
                            response = responseData,
                            remoteip = remoteIpAddress
                        });

                        dynamic content = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(result));
                        valid = content.success;
                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorLog.GetDefault(httpContext.ApplicationInstance.Context).Log(new Elmah.Error(e));
                }
            }
            return valid;
        }

        public class RecaptchaVerification : NameValueCollection
        {
            public string remoteip
            {
                get { return this["remoteip"]; }
                set { this["remoteip"] = value; }
            }
            public string response
            {
                get { return this["response"]; }
                set { this["response"] = value; }
            }
            public string secret
            {
                get { return this["secret"]; }
                set { this["secret"] = value; }
            }
        }
    }
}
