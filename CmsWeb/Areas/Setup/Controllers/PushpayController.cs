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
        [Route("~/Pushpay/{APITest}")]
        [HttpGet]
        public JsonResult Index(bool APITest)
        {
            string redirectUrl = new MultipleGatewayUtils(CurrentDatabase).Setting("OAuth2AuthorizeEndpoint", "", 1)
                + "?client_id=" + new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayClientID", "", 1)
                + "&response_type=code"
                + "&redirect_uri=" + new MultipleGatewayUtils(CurrentDatabase).Setting("TouchpointAuthServer", "", 1)
                + "&scope=" + new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayScope", "", 1)
                + "&state=" + CurrentDatabase.Host; //Get  xsrf_token:tenantID

            return Json(redirectUrl, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous, Route("~/Pushpay/Complete")]
        public async Task<ActionResult> Complete()
        {
            string redirectUrl;
            var tenantHost = Request["state"];
            if (!new MultipleGatewayUtils(CurrentDatabase).Setting("IsDeveloperMode", 1))
            {
                redirectUrl = "https://" + tenantHost + "." + new MultipleGatewayUtils(CurrentDatabase).Setting("OrgBaseDomain", "", 1) + "/Pushpay/Save";
            }
            else
            {
                redirectUrl = "http://" + new MultipleGatewayUtils(CurrentDatabase).Setting("TenantHostDev", "", 1) + "/Pushpay/Save";
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
                { "client_id", new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayClientID", "", 1)}
                ,{"client_secret", new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayClientSecret", "", 1)}
                ,{"grant_type", "authorization_code"}
                ,{"code", _authCode}
                ,{"redirect_uri", new MultipleGatewayUtils(CurrentDatabase).Setting("TouchpointAuthServer", "", 1)}
            };

            var client = new HttpClient();
            //Setting a "basic auth" header
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayClientID", "", 1), new MultipleGatewayUtils(CurrentDatabase).Setting("PushpayClientSecret", "", 1))
                        )));
            var postContent = new FormUrlEncodedContent(post);
            var response = await client.PostAsync(new MultipleGatewayUtils(CurrentDatabase).Setting("OAuth2TokenEndpoint", "", 1), postContent);
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
