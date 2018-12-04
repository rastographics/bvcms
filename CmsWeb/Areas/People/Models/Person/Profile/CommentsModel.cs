using CmsData;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CmsWeb.Areas.People.Models
{
    public class CommentsModel
    {
        public CommentsModel(int id)
        {
            Comments = (from p in DbUtil.Db.People
                        where p.PeopleId == id
                        select p.Comments).Single();
            PeopleId = id;
        }

        public CommentsModel()
        {
        }

        public int PeopleId { get; set; }

        [UIHint("Textarea")]
        public string Comments { get; set; }

        public void UpdateComments()
        {
            var p = DbUtil.Db.LoadPersonById(PeopleId);
            p.Comments = Comments;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"Updated Comments: {p.Name}");
        }
    }
}
