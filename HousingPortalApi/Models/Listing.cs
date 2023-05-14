using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingPortalApi.Models
{
    public class Listing : AuditEntity
    {
        [Key]
        public Guid listingId { get; set; }

        [StringLength(100)]
        public string title { get; set; }

        [StringLength(1000)]
        public string description { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        [StringLength(100)]
        public string city { get; set; }
        [Required]
        [StringLength(100)]
        public string state { get; set; }
        public string zip { get; set; }
        public string image { get; set; }

        [ForeignKey("Student")]
        public Guid studentId { get; set; }
        public virtual Student student { get; set; }
 
    }
}
