namespace CmsWeb.Areas.People.Models
{
    public class PersonDocumentsModel
    {
        public int PeopleId { get; set; }
        public bool Finance { get; set; }
        public bool CanEdit { get; set; }
        public string Title { get; set; }
    }
}
