using System;
using System.Collections.Generic;

namespace PatientManagementApp.DTOs
{
	public class PatientDto
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public List<ContactInfoDto> ContactInfos { get; set; }
		public AddressDto PrimaryAddress { get; set; }
		public AddressDto? SecondaryAddress { get; set; }
		public bool IsActive { get; set; }
		public string? InactiveReason { get; set; }
	}
}
