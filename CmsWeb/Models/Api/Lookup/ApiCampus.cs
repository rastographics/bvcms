using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    [ApiMapName("Campuses")]
    public class ApiCampus
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
