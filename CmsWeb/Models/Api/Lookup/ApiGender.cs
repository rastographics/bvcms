using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    [ApiMapName("Genders")]
    public class ApiGender
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
