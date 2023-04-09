using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingPortalApi.Models
{
    public class Listing
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }

        [StringLength(15)]
        public string Contact { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Guid StudentId { get; set; }

        [ForeignKey("StudentId")]
        [InverseProperty("Listings")]
        public virtual Student Student { get; set; }
    }
}
