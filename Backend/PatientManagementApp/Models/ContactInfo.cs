using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientManagementApp.Models
{
    public class ContactInfo
    {
        [Key]
        public int ContactInfoId { get; set; }

        [Required]
        public string Type { get; set; } // "Phone", "Email"

        [Required]
        public string Value { get; set; }

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
