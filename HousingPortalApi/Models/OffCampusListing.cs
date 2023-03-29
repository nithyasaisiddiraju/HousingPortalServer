using System.ComponentModel.DataAnnotations;

namespace HousingPortalApi.Models
{
    public class OffCampusListing
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Contact { get; set; }

    }
}
