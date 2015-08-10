using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    [ApiMapName("ContributionTypes")]
    public class ApiContributionType
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
