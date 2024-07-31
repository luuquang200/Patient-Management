using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientManagementApp.Models
{

	public class Patient
	{
		[Key]
		public int PatientId { get; set; }

		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		public string LastName { get; set; }

		[Required]
		public string Gender { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }

		public bool IsActive { get; set; } = true;

		[MaxLength(255)]
		public string? InactiveReason { get; set; }

		public virtual ICollection<ContactInfo> ContactInfos { get; set; }

		[Required]
		public virtual Address PrimaryAddress { get; set; }

		public virtual Address? SecondaryAddress { get; set; }
	}
}
