using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingPortalApi.Models
{
    public class Student : AuditEntity
    {
        [Key]
        public Guid studentId { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string phone { get; set; }

        [Required]
        [StringLength(100)]
        public string major { get; set; }

        [Required]
        [Range(1000, 9999)]
        public int graduationYear { get; set; }

        [InverseProperty("student")]
        public virtual ICollection<Listing> listings { get; set; } = new List<Listing>();
    }
}