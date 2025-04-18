﻿using System.ComponentModel.DataAnnotations;

namespace PatientManagementApp.Models
{
	public class Address
	{
		[Key]
		public int AddressId { get; set; }

		[Required]
		public string Street { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string ZipCode { get; set; }

		[Required]
		public string Country { get; set; }
	}
}
