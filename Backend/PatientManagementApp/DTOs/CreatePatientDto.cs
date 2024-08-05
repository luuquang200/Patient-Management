using FluentValidation;

namespace PatientManagementApp.DTOs
{
	public class CreatePatientDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public List<ContactInfoDto> ContactInfos { get; set; }
		public AddressDto PrimaryAddress { get; set; }
		public AddressDto? SecondaryAddress { get; set; }
	}

	public class CreatePatientDtoValidator : AbstractValidator<CreatePatientDto>
	{
		public CreatePatientDtoValidator()
		{
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.")
				.MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.")
				.MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

			RuleFor(x => x.Gender)
				.NotEmpty().WithMessage("Gender is required.")
				.Must(g => g == "Male" || g == "Female" || g == "Other")
				.WithMessage("Gender must be either Male, Female, or Other.");

			RuleFor(x => x.DateOfBirth)
				.NotEmpty().WithMessage("Date of birth is required.")
				.LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");

			RuleFor(x => x.ContactInfos)
				.NotEmpty().WithMessage("At least one contact info is required.")
				.Must(c => c.Count > 0).WithMessage("At least one contact info is required.");

			RuleForEach(x => x.ContactInfos).SetValidator(new ContactInfoDtoValidator());

			RuleFor(x => x.PrimaryAddress)
				.NotNull().WithMessage("Primary address is required.")
				.SetValidator(new AddressDtoValidator());

			RuleFor(x => x.SecondaryAddress)
				.SetValidator(new NullableAddressDtoValidator())
				.When(x => x.SecondaryAddress != null);
		}
	}

	public class ContactInfoDtoValidator : AbstractValidator<ContactInfoDto>
	{
		public ContactInfoDtoValidator()
		{
			RuleFor(x => x.Type)
				.NotEmpty().WithMessage("Contact type is required.")
				.MaximumLength(20).WithMessage("Contact type must not exceed 20 characters.");

			RuleFor(x => x.Value)
				.NotEmpty().WithMessage("Contact value is required.")
				.MaximumLength(50).WithMessage("Contact value must not exceed 50 characters.");
		}
	}

	public class AddressDtoValidator : AbstractValidator<AddressDto>
	{
		public AddressDtoValidator()
		{
			RuleFor(x => x.Street)
				.NotEmpty().WithMessage("Street is required.")
				.MaximumLength(100).WithMessage("Street must not exceed 100 characters.");

			RuleFor(x => x.City)
				.NotEmpty().WithMessage("City is required.")
				.MaximumLength(50).WithMessage("City must not exceed 50 characters.");

			RuleFor(x => x.State)
				.NotEmpty().WithMessage("State is required.")
				.MaximumLength(50).WithMessage("State must not exceed 50 characters.");

			RuleFor(x => x.ZipCode)
				.NotEmpty().WithMessage("Zip code is required.")
				.MaximumLength(20).WithMessage("Zip code must not exceed 20 characters.");

			RuleFor(x => x.Country)
				.NotEmpty().WithMessage("Country is required.")
				.MaximumLength(50).WithMessage("Country must not exceed 50 characters.");
		}
	}

	public class NullableAddressDtoValidator : AbstractValidator<AddressDto?>
	{
		public NullableAddressDtoValidator()
		{
			When(x => x != null, () =>
			{
				RuleFor(x => x.Street)
					.NotEmpty().WithMessage("Street is required.")
					.MaximumLength(100).WithMessage("Street must not exceed 100 characters.");

				RuleFor(x => x.City)
					.NotEmpty().WithMessage("City is required.")
					.MaximumLength(50).WithMessage("City must not exceed 50 characters.");

				RuleFor(x => x.State)
					.NotEmpty().WithMessage("State is required.")
					.MaximumLength(50).WithMessage("State must not exceed 50 characters.");

				RuleFor(x => x.ZipCode)
					.NotEmpty().WithMessage("Zip code is required.")
					.MaximumLength(20).WithMessage("Zip code must not exceed 20 characters.");

				RuleFor(x => x.Country)
					.NotEmpty().WithMessage("Country is required.")
					.MaximumLength(50).WithMessage("Country must not exceed 50 characters.");
			});
		}
	}
}
