using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models.iPhone;
using CmsWeb.MobileAPI;
using UtilityExtensions;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIController : Controller
	{
		public ActionResult Authorize()
		{
			var authError = Authenticate();
			if (authError != null) return authError;
			
			BaseReturn br = new BaseReturn();
			br.error = 0;
			br.id = Util.UserPeopleId ?? 0;

			List<MobilePerson> mp = new List<MobilePerson>();

			var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == Util.UserPeopleId);

			if (person != null)
			{
				mp.Add(new MobilePerson().populate(person));
				br.data = JsonConvert.SerializeObject(mp);
				br.count = 1;
			}

			return br;
		}

		private ActionResult Authenticate()
		{
			if (CmsWeb.Models.AccountModel.AuthenticateMobile()) return null;
			else
			{
				BaseReturn br = new BaseReturn();
				br.data = "You are not authorized!";
				return br;
			}
		}

		public ActionResult DoSearch(string name, string comm, string addr)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobilePerson> mp = new List<MobilePerson>();

			var m = new SearchModel(name, comm, addr);

			br.error = 0;
			br.type = 1;
			br.count = m.Count;

			foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(20))
			{
				mp.Add(new MobilePerson().populate(item));
			}

			br.data = JsonConvert.SerializeObject(mp);
			return br;
		}

		public ActionResult FetchPerson(int id)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			List<MobilePerson> mp = new List<MobilePerson>();

			var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == id);

			if (person == null)
			{
				br.error = 1;
				br.data = "Person not found.";
				return br;
			}

			br.error = 0;
			br.type = 1;
			br.count = 1;

			mp.Add(new MobilePerson().populate(person));

			br.data = JsonConvert.SerializeObject(mp);

			return br;
		}

		public ActionResult FetchImage( int id, int size )
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();
			if( id == 0 ) return br.setData("The ID for the person cannot be set to zero");

			br.data = "The picture was not found.";

			var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == id);

			if (person.PictureId != null)
			{
				ImageData.Image image = null;

				switch (size)
				{
					case 0: // 50 x 50
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId);
						break;

					case 1: // 120 x 120
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);
						break;

					case 2: // 320 x 400
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId);
						break;

					case 3: // 570 x 800
						image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId);
						break;

				}

				if (image != null)
				{
					br.data = Convert.ToBase64String(image.Bits);
					br.count = 1;
					br.error = 0;
				}
			}

			return br;
		}

		public ActionResult SaveImage(int id, string image)
		{
			var authError = Authenticate();
			if (authError != null) return authError;

			BaseReturn br = new BaseReturn();

			var imageBytes = Convert.FromBase64String(image);

			var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == id);

			if (person.Picture != null)
			{
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.ThumbId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.SmallId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.MediumId).SingleOrDefault());
				ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.Where(i => i.Id == person.Picture.LargeId).SingleOrDefault());

				person.Picture.ThumbId = ImageData.Image.NewImageFromBits(imageBytes, 50, 50).Id;
				person.Picture.SmallId = ImageData.Image.NewImageFromBits(imageBytes, 120, 120).Id;
				person.Picture.MediumId = ImageData.Image.NewImageFromBits(imageBytes, 320, 400).Id;
				person.Picture.LargeId = ImageData.Image.NewImageFromBits(imageBytes, 570, 800).Id;
			}
			else
			{
				var newPicture = new Picture();

				newPicture.ThumbId = ImageData.Image.NewImageFromBits( imageBytes, 50, 50 ).Id;
				newPicture.SmallId = ImageData.Image.NewImageFromBits( imageBytes, 120, 120 ).Id;
				newPicture.MediumId = ImageData.Image.NewImageFromBits( imageBytes, 320, 400 ).Id;
				newPicture.LargeId = ImageData.Image.NewImageFromBits( imageBytes, 570, 800 ).Id;

				person.Picture = newPicture;
			}

			DbUtil.Db.SubmitChanges();

			br.error = 0;
			br.data = "Image updated.";
			br.id = id;
			br.count = 1;

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
		public int error = 1;
		public int type = 0;
		public int count = 0;
		public int id = 0;
		public string data = "";

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write(JsonConvert.SerializeObject(this));
		}

		public BaseReturn setData(string newData)
		{
			data = newData;
			return this;
		}
	}
}