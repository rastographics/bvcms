using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CmsWeb.Common;
using CmsData;
using System.Net.Http;
using System.Net.Http.Headers;
using CmsWeb.Pushpay.Entities;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Pushpay")]
    public class PushpayController : CmsStaffController
    {        
        
        /// <summary>
        ///     Opens the developer console in a separate VIEW
        /// </summary>
        /// <returns></returns>
        public ActionResult DeveloperConsole()
        {
            return View();
        }

        /// <summary>
        ///     Entry point / home page into the application
        /// </summary>
        /// <returns></returns>
        [Route("~/Pushpay")]
        public ActionResult Index()
        {            
            string redirectUrl = Configuration.Current.OAuth2AuthorizeEndpoint
                + "?client_id=" + Configuration.Current.PushpayClientID
                + "&response_type=code"
                + "&redirect_uri=" + Configuration.Current.TouchpointAuthServer
                + "&scope=" + Configuration.Current.PushpayScope
                + "&state=" + DbUtil.Db.Host; //Get  xsrf_token:tenantID

            return Redirect(redirectUrl);
        }

        [AllowAnonymous, Route("~/Pushpay/Complete")]
        public async Task<ActionResult> Complete()
        {
            string redirectUrl;
            if (!Configuration.Current.IsDeveloperMode) {
                redirectUrl = "https://" + DbUtil.Db.Host + "." + Configuration.Current.OrgBaseDomain + "/Pushpay/Save";
            }
            else {
                redirectUrl = "https://" + Configuration.Current.TenantHostDev + "/Pushpay/Save";
            }

            //Get code returned from Pushpay
            AccessToken at = await AuthorizationCodeCallback();
            
            return Redirect(redirectUrl+"?_at=" + at.access_token + "&_rt=" + at.refresh_token );
        }

        [Route("~/Pushpay/Save")]
        public ActionResult Save(string _at, string _rt)
        {
            string idAccessToken = "PushpayAccessToken", idRefreshToken= "PushpayRefreshToken";
            var dbContext = DbUtil.Db;
            var m = dbContext.Settings.AsQueryable();
            if (!Regex.IsMatch(idAccessToken, @"\A[A-z0-9-]*\z"))
                return Message("Invalid characters in setting id");

            if (!dbContext.Settings.Any(s => s.Id == idAccessToken))
            {
                var s = new Setting { Id = idAccessToken, SettingX = _at};
                dbContext.Settings.InsertOnSubmit(s);
                dbContext.SubmitChanges();
                dbContext.SetSetting(idAccessToken, _at);
            }
            if (!dbContext.Settings.Any(s => s.Id == idRefreshToken))
            {
                var s = new Setting { Id = idRefreshToken, SettingX = _rt };
                dbContext.Settings.InsertOnSubmit(s);
                dbContext.SubmitChanges();
                dbContext.SetSetting(idRefreshToken, _rt);
            }
            
            return RedirectToAction("Finish");
        }

        [Route("~/Pushpay/Finish")]
        public ActionResult Finish()
        { return View();  }

        public async Task<AccessToken> AuthorizationCodeCallback()
        {            
            // received authorization code from authorization server
            var authorizationCode = Request["code"];
                       
            // exchange authorization code at authorization server for an access and refresh token
            Dictionary<string, string> post = null;
            post = new Dictionary<string, string>
            {
                { "client_id", Configuration.Current.PushpayClientID}
                ,{"client_secret", Configuration.Current.PushpayClientSecret}
                ,{"grant_type", "authorization_code"}
                ,{"code", authorizationCode}
                ,{"redirect_uri", Configuration.Current.TouchpointAuthServer}
            };

            var client = new HttpClient();
            //Setting a "basic auth" header
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", Configuration.Current.PushpayClientID, Configuration.Current.PushpayClientSecret)
                        )));            
            var postContent = new FormUrlEncodedContent(post);
            var response = await client.PostAsync(Configuration.Current.OAuth2TokenEndpoint, postContent);
            var content = await response.Content.ReadAsStringAsync();
            var _accessToken = new AccessToken();
            // exchange code for tokens from authorization server
            try {                
                var json = JObject.Parse(content);                
                _accessToken.access_token = json["access_token"].ToString();
                _accessToken.token_type = json["token_type"].ToString();
                _accessToken.expires_in = Convert.ToInt64(json["expires_in"].ToString());
                if (json["refresh_token"] != null)
                    _accessToken.refresh_token = json["refresh_token"].ToString();
            }
            catch (Exception ex) {
                ModelState.AddModelError("form", ex.Message);                
            }            
            if (_accessToken != null)
                return _accessToken;
            else return null;                        
        }
        
    }
}
