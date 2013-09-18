using System.Collections.Generic;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public interface ITasks
    {
        PagerModel2 Pager { get; set; }
        IEnumerable<TaskInfo> Tasks();
        string AddTask { get; set; }
    }
}