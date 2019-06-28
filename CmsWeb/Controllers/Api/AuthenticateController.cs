using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using UtilityExtensions;

namespace CmsWeb.Controllers.Api
{
    public class AuthenticateController : ApiController
    {
        //todo: inheritance chain
        private readonly RequestManager _requestManager;
        private CMSDataContext CurrentDatabase => _requestManager.CurrentDatabase;

        public AuthenticateController(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        [HttpGet, Route("~/API/Authenticate/OneTimeLink/{otltoken}")]
        public object ValidateOneTimeLink(string otltoken)
        {
            var code = otltoken.ToGuid();
            if (!code.HasValue)
            {
                throw new Exception("Bad otltoken format");
            }

            var otlink = CurrentDatabase.OneTimeLinks.FirstOrDefault(m => m.Id == code && m.Used == false && m.Expires > DateTime.Now);
            if (otlink == null)
            {
                return NotFound();
            }
            else
            {
                otlink.Used = true;
                CurrentDatabase.SubmitChanges();

                var peopleId = otlink.Querystring.ToInt();
                var person = CurrentDatabase.People.FirstOrDefault(p => p.PeopleId == peopleId);
                if (person == null)
                {
                    return NotFound();
                }
                else
                {
                    return new {
                        person.PeopleId,
                        person.FirstName,
                        person.PreferredName,
                        person.LastName,
                        person.EmailAddress,
                        person.EmailAddress2,
                        person.CampusId
                    };
                }
            }
        }
    }
}
