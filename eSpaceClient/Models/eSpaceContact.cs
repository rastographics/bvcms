namespace eSpace.Models
{
    public class eSpaceContact : eSpacePerson
    {
        public long ContactId { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
