using PatientManagementApp.DTOs;
using PatientManagementApp.Models;

namespace PatientManagementApp.Services
{
	public static class PatientMapper
	{
		public static PatientDto MapToPatientDto(Patient patient)
		{
			return new PatientDto
			{
				Id = patient.PatientId,
				FirstName = patient.FirstName,
				LastName = patient.LastName,
				Gender = patient.Gender,
				DateOfBirth = patient.DateOfBirth.Date,
				ContactInfos = patient.ContactInfos.Select(c => new ContactInfoDto
				{
					Type = c.Type,
					Value = c.Value
				}).ToList(),
				PrimaryAddress = new AddressDto
				{
					Street = patient.PrimaryAddress.Street,
					City = patient.PrimaryAddress.City,
					State = patient.PrimaryAddress.State,
					ZipCode = patient.PrimaryAddress.ZipCode,
					Country = patient.PrimaryAddress.Country
				},
				SecondaryAddress = patient.SecondaryAddress != null ? new AddressDto
				{
					Street = patient.SecondaryAddress.Street,
					City = patient.SecondaryAddress.City,
					State = patient.SecondaryAddress.State,
					ZipCode = patient.SecondaryAddress.ZipCode,
					Country = patient.SecondaryAddress.Country
				} : null,
				IsActive = patient.IsActive,
				InactiveReason = patient.InactiveReason
			};
		}

		public static Patient MapToEntity(CreatePatientDto createPatientDto)
		{
			return new Patient
			{
				FirstName = createPatientDto.FirstName,
				LastName = createPatientDto.LastName,
				Gender = createPatientDto.Gender,
				DateOfBirth = createPatientDto.DateOfBirth.Date,
				ContactInfos = createPatientDto.ContactInfos.Select(c => new ContactInfo
				{
					Type = c.Type,
					Value = c.Value
				}).ToList(),
				PrimaryAddress = new Address
				{
					Street = createPatientDto.PrimaryAddress.Street,
					City = createPatientDto.PrimaryAddress.City,
					State = createPatientDto.PrimaryAddress.State,
					ZipCode = createPatientDto.PrimaryAddress.ZipCode,
					Country = createPatientDto.PrimaryAddress.Country
				},
				SecondaryAddress = createPatientDto.SecondaryAddress != null ? new Address
				{
					Street = createPatientDto.SecondaryAddress.Street,
					City = createPatientDto.SecondaryAddress.City,
					State = createPatientDto.SecondaryAddress.State,
					ZipCode = createPatientDto.SecondaryAddress.ZipCode,
					Country = createPatientDto.SecondaryAddress.Country
				} : null
			};
		}
	}
}
