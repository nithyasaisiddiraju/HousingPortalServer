using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingPortalApi.Models
{
    public class Listing : AuditEntity
    {
        [Key]
        public Guid ListingId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [StringLength(100)]
        public string State { get; set; }
        public string Zip { get; set; }
        public string Image { get; set; }

        [ForeignKey("Student")]
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
 
    }
}
