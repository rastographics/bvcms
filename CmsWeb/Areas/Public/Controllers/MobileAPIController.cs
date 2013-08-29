using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models.iPhone;
using CmsWeb.MobileAPI;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIController : Controller
	{
		public ActionResult Authorize()
		{
			BaseReturn br = new BaseReturn();

			if (CmsWeb.Models.AccountModel.AuthenticateMobile())
			{
				br.id = Util.UserPeopleId ?? 0;
				return br;
			}
			else
			{
				br.error = 1;
				br.data = "Username and password combination not found, please try again.";
				return br;
			}
		}

		private ActionResult Authenticate()
		{
			if (CmsWeb.Models.AccountModel.AuthenticateMobile()) return null;
			else
			{
				BaseReturn br = new BaseReturn();

				br.error = 1;
				br.data = "You are not authorized!";
				return br;
			}
		}

		public ActionResult Search(string name, string comm, string addr)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobilePerson> mp = new List<MobilePerson>();

			var m = new SearchModel(name, comm, addr);

			br.type = 1;
			br.count = m.Count;

			foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(20))
			{
				mp.Add(new MobilePerson().populate(item));
			}

			br.data = JSONHelper.JsonSerializer<List<MobilePerson>>(mp);
			return br;
		}

		public ActionResult TaskList(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTask> mt = new List<MobileTask>();

			var list = from e in DbUtil.Db.Tasks
						  where e.OwnerId == ID
						  select e;

			br.type = 101;

			if (list != null)
			{
				br.count = list.Count();

				foreach (var item in list)
				{
					mt.Add(new MobileTask().populate(item));
				}

				br.data = JSONHelper.JsonSerializer<List<MobileTask>>(mt);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskItem(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			MobileTask mt = new MobileTask();

			var item = (from e in DbUtil.Db.Tasks
							where e.Id == ID
							select e).SingleOrDefault();

			br.type = 102;

			if (item != null)
			{
				br.count = 1;

				mt.populate(item);
				br.data = JSONHelper.JsonSerializer<MobileTask>(mt);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskBoxList(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTaskBox> mtb = new List<MobileTaskBox>();

			var list = from e in DbUtil.Db.TaskLists
						  where e.CreatedBy == ID
						  select e;

			br.type = 103;

			if (list != null)
			{
				br.count = list.Count();

				foreach (var item in list)
				{
					mtb.Add(new MobileTaskBox().populate(item));
				}

				br.data = JSONHelper.JsonSerializer<List<MobileTaskBox>>(mtb);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskBoxItem(int ID)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			MobileTaskBox mtb = new MobileTaskBox();

			var item = (from e in DbUtil.Db.TaskLists
							where e.CreatedBy == ID
							select e).SingleOrDefault();

			br.type = 104;

			if (item != null)
			{
				br.count = 1;

				mtb.populate(item);
				br.data = JSONHelper.JsonSerializer<MobileTaskBox>(mtb);
			}
			else
			{
				br.error = 1;
			}

			return br;
		}

		public ActionResult TaskStatusList()
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobileTaskStatus> ls = new List<MobileTaskStatus>();

			br.type = 105;

			var s = from e in DbUtil.Db.TaskStatuses
					  select e;

			foreach (var item in s)
			{
				ls.Add(new MobileTaskStatus().populate(item));
			}

			br.count = s.Count();
			br.data = JSONHelper.JsonSerializer<List<MobileTaskStatus>>(ls);

			return br;
		}

		[ValidateInput(false)]
		public ActionResult TaskCreate(string type, string data) // Type 1001
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			br.type = 1001;

			MobileTask mt = JSONHelper.JsonDeserialize<MobileTask>(data);
			if (mt != null) br.id = mt.addToDB();
			else
			{
				br.error = 1;
				br.data = "Task was not created.";
			}

			return br;
		}

		[ValidateInput(false)]
		public ActionResult TaskUpdate(string type, string data) // Type 1002
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			br.type = 1002;

			MobileTask mt = JSONHelper.JsonDeserialize<MobileTask>(data);

			var t = from e in DbUtil.Db.Tasks
					  where e.Id == mt.id
					  select e;

			if (t != null)
			{
				var task = t.Single();

				if (mt.updateDue > 0) task.Due = mt.due;
				if (mt.statusID > 0) task.StatusId = mt.statusID;
				if (mt.priority > 0) task.Priority = mt.priority;
				if (mt.notes.Length > 0) task.Notes = mt.notes;
				if (mt.description.Length > 0) task.Description = mt.description;
				if (mt.ownerID > 0) task.OwnerId = mt.ownerID;
				if (mt.boxID > 0) task.ListId = mt.boxID;
				if (mt.aboutID > 0) task.WhoId = mt.aboutID;
				if (mt.delegatedID > 0) task.CoOwnerId = mt.delegatedID;
				if (mt.notes.Length > 0) task.Notes = mt.notes;

				DbUtil.Db.SubmitChanges();

				br.data = "Task updated.";
			}
			else
			{
				br.error = 1;
				br.data = "Task not found.";
			}

			return br;
		}
	}

	public class BaseReturn : ActionResult
	{
		public int error = 0;
		public int type = 0;
		public int count = 0;
		public int id = 0;
		public string data = "";

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write(JSONHelper.JsonSerializer<BaseReturn>(this));
		}
	}
}