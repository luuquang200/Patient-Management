using FluentValidation;

namespace PatientManagementApp.DTOs
{
	public class UpdatePatientDto : CreatePatientDto
	{
		public Guid Id { get; set; }
	}

	public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto>
	{
		public UpdatePatientDtoValidator()
		{
			// Reuse the CreatePatientDtoValidator rules
			Include(new CreatePatientDtoValidator());

			// Additional validation rules for UpdatePatientDto
			RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
		}
	}
}
