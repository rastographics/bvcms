using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
    public class MobileTaskList
    {
        public List<MobileTask> pending = new List<MobileTask>();
        public List<MobileTask> active = new List<MobileTask>();
        public List<MobileTask> delegated = new List<MobileTask>();

        public void addTask(MobileTask task, int currentPeopleID)
        {
            if (currentPeopleID == 0) return;

            if (task.delegatedID > 0 && task.delegatedID != currentPeopleID)
            {
                task.type = MobileTask.TYPE_DELEGATED;
                delegated.Add(task);
                return;
            }

            if (task.statusID == 10)
            {
                task.type = MobileTask.TYPE_ACTIVE;
                active.Add(task);
            }
            else
            {
                task.type = MobileTask.TYPE_PENDING;
                pending.Add(task);
            }
        }
    }
}