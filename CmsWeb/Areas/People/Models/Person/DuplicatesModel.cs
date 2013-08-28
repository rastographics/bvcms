using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class DuplicatesModel
    {
        public CmsData.Person person { get; set; }
        public DuplicatesModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
        }
    }
}
