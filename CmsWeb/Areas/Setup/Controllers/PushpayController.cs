using CmsData;
using CmsWeb.Common;
using CmsWeb.Lifecycle;
using CmsWeb.Pushpay.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Pushpay")]
    public class PushpayController : Controller
    {
        //todo: Inheritance chain
        private readonly RequestManager RequestManager;
        private CMSDataContext CurrentDatabase => RequestManager.CurrentDatabase;

        public PushpayController(RequestManager requestManager)
        {
            RequestManager = requestManager;
        }

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
                + "&state=" + CurrentDatabase.Host; //Get  xsrf_token:tenantID

            return Redirect(redirectUrl);
        }

        [AllowAnonymous, Route("~/Pushpay/Complete")]
        public async Task<ActionResult> Complete()
        {
            string redirectUrl;
            var tenantHost = Request["state"];
            if (!Configuration.Current.IsDeveloperMode)
            {
                redirectUrl = "https://" + tenantHost + "." + Configuration.Current.OrgBaseDomain + "/Pushpay/Save";
            }
            else
            {
                redirectUrl = "http://" + Configuration.Current.TenantHostDev + "/Pushpay/Save";
            }

            //Received authorization code from authorization server
            var authorizationCode = Request["code"];
            if (authorizationCode != null && authorizationCode != "")
            {
                //Get code returned from Pushpay
                var at = await AuthorizationCodeCallback(authorizationCode);
                return Redirect(redirectUrl + "?_at=" + at.access_token + "&_rt=" + at.refresh_token);
            }
            return Redirect("~/Home/Index");
        }

        [Route("~/Pushpay/Save")]
        public ActionResult Save(string _at, string _rt)
        {
            string idAccessToken = "PushpayAccessToken", idRefreshToken = "PushpayRefreshToken";
            //var dbContext = Db;
            //var m = CurrentDatabase.Settings.AsQueryable();
            if (!Regex.IsMatch(idAccessToken, @"\A[A-z0-9-]*\z"))
            {
                return View("Invalid characters in setting id");
            }

            if (!CurrentDatabase.Settings.Any(s => s.Id == idAccessToken))
            {
                //Create access token
                var s = new Setting { Id = idAccessToken, SettingX = _at };
                CurrentDatabase.Settings.InsertOnSubmit(s);
                CurrentDatabase.SubmitChanges();
                CurrentDatabase.SetSetting(idAccessToken, _at);
            }
            else
            { // Update access token
                CurrentDatabase.SetSetting(idAccessToken, _at);
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity($"Edit Setting {idAccessToken} to {_at}", userId: Util.UserId);
            }
            if (!CurrentDatabase.Settings.Any(s => s.Id == idRefreshToken))
            { //Create refresh token
                var s = new Setting { Id = idRefreshToken, SettingX = _rt };
                CurrentDatabase.Settings.InsertOnSubmit(s);
                CurrentDatabase.SubmitChanges();
                CurrentDatabase.SetSetting(idRefreshToken, _rt);
            }
            else
            { // Update refresh token
                CurrentDatabase.SetSetting(idRefreshToken, _rt);
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity($"Edit Setting {idRefreshToken} to {_rt}", userId: Util.UserId);
            }

            return RedirectToAction("Finish");
        }

        [Route("~/Pushpay/Finish")]
        public ActionResult Finish()
        { return View(); }

        public async Task<AccessToken> AuthorizationCodeCallback(string _authCode)
        {


            // exchange authorization code at authorization server for an access and refresh token
            Dictionary<string, string> post = null;
            post = new Dictionary<string, string>
            {
                { "client_id", Configuration.Current.PushpayClientID}
                ,{"client_secret", Configuration.Current.PushpayClientSecret}
                ,{"grant_type", "authorization_code"}
                ,{"code", _authCode}
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
            try
            {
                var json = JObject.Parse(content);
                _accessToken.access_token = json["access_token"].ToString();
                _accessToken.token_type = json["token_type"].ToString();
                _accessToken.expires_in = Convert.ToInt64(json["expires_in"].ToString());
                if (json["refresh_token"] != null)
                {
                    _accessToken.refresh_token = json["refresh_token"].ToString();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("form", ex.Message);
            }
            if (_accessToken != null)
            {
                return _accessToken;
            }
            else
            {
                return null;
            }
        }

    }
}
