using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    [ApiMapName("MaritalStatuses")]
    public class ApiMaritalStatus
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
