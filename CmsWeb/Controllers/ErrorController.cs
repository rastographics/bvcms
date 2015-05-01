using System;
using System.Web.Mvc;

namespace CmsWeb.Controllers
{
    /// <summary>
    /// NOTE:  All of the HTTP 4* and 5* errors below are decorated as accepting any of the allowable HTTP verbs
    /// (see the full list at http://en.wikipedia.org/wiki/Hypertext_Transfer_Protocol). The reason for this is
    /// that our error handling performs a Server.TransferRequest so that users can just hit F5 to refresh any 
    /// pages with errors instead of being redirected. This is fine unless the user just performed an HTTP POST 
    /// to the server - the transfer request also transfers the verb action, so MVC has to accept any potential 
    /// HTTP verb here so that the error can correctly propagate back to the user instead of receiving an HTTP 404.
    /// </summary>
    public class ErrorController : Controller
    {

        [AcceptVerbs("HEAD", "GET", "POST", "PUT", "DELETE", "TRACE", "OPTIONS", "CONNECT", "PATCH")]
        [ActionName("500")]
        public ActionResult Index(string errorMessage)
        {
            Response.TrySkipIisCustomErrors = true;

            if (!Request.IsAjaxRequest())
            {
                Response.StatusCode = 500;
            }

            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        [AcceptVerbs("HEAD", "GET", "POST", "PUT", "DELETE", "TRACE", "OPTIONS", "CONNECT", "PATCH")]
        [ActionName("400")]
        public ActionResult BadRequest()
        {
            Response.TrySkipIisCustomErrors = true;

            if (!Request.IsAjaxRequest())
            {
                Response.StatusCode = 400;
            }

            return View();
        }

        [AcceptVerbs("HEAD", "GET", "POST", "PUT", "DELETE", "TRACE", "OPTIONS", "CONNECT", "PATCH")]
        [ActionName("401")]
        public ActionResult UnAuthorized()
        {
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        [AcceptVerbs("HEAD", "GET", "POST", "PUT", "DELETE", "TRACE", "OPTIONS", "CONNECT", "PATCH")]
        [ActionName("404")]
        public ActionResult NotFound()
        {
            Response.TrySkipIisCustomErrors = true;

            if (!Request.IsAjaxRequest())
            {
                Response.StatusCode = 404;
            }

            return View();
        }

        public ActionResult DatabaseCreationError(string error)
        {
            ViewBag.ErrorMessage = error;
            return View();
        }

        public ActionResult DatabaseNotFound(string dbname)
        {
            ViewBag.DbName = dbname;
            return View();
        }

        public ActionResult DatabaseNotInitialized(string dbname)
        {
            ViewBag.DbName = dbname;
            return View();
        }

        public ActionResult DatabaseServerNotFound(string server)
        {
            ViewBag.Server = server;
            return View();
        }

        public ActionResult Offline()
        {
            return View();
        }

        public ActionResult SessionTimeout()
        {
            return View();
        }

        /// <summary>
        /// This is a temp view for the new user interface feature.
        /// TODO:// removed this action and view once we go to production.
        /// </summary>
        /// <returns></returns>
        public ActionResult UnderConstruction()
        {
            return View();
        }
    }
}