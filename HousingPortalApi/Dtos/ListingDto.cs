using HousingPortalApi.Models;

namespace HousingPortalApi.Dtos
{
    public class ListingDto
    {
        public Guid ListingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Image { get; set; }
        public StudentDto StudentDto { get; set; }
    }


}
