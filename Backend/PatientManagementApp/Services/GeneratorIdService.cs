using System;
using System.Threading;

namespace PatientManagementApp.Services
{
	public interface IGeneratorIdService
	{
		Guid GenerateId();
	}
	public class GeneratorIdService : IGeneratorIdService
	{
		public Guid GenerateId()
		{
			return Guid.NewGuid();
		}
	}
}