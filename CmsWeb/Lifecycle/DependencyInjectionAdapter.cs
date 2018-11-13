using CmsWeb.Areas.People.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Lifecycle
{
    public interface IDbUtilAdapter { }

    public class DbUtilAdapter : IDbUtilAdapter
    {

        public PersonModel GetPersonModelById(int id)
        {

        }
    }
}
