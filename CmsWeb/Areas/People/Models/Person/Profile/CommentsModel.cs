using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class CommentsModel
    {
        public CommentsModel(int id)
        {
            Comments = (from p in CurrentDatabase.People
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
            var p = CurrentDatabase.LoadPersonById(PeopleId);
            p.Comments = Comments;
            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"Updated Comments: {p.Name}");
        }
    }
}
