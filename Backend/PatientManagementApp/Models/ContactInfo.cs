using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
		public int PatientId { get; set; }

		public virtual Patient Patient { get; set; }
	}
}
