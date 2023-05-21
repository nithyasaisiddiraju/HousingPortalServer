using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingPortalApi.Models
{
    public class Student : AuditEntity
    {
        [Key]
        public Guid StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [StringLength(100)]
        public string Major { get; set; }

        [Required]
        [Range(1000, 9999)]
        public int GraduationYear { get; set; }

        [InverseProperty("Student")]
        public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}