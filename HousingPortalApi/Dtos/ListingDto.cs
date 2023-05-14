using HousingPortalApi.Models;

namespace HousingPortalApi.Dtos
{
    public class ListingDto
    {
        public Guid listingId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public decimal price { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string image { get; set; }
        public StudentDto studentDto { get; set; }
    }


}
