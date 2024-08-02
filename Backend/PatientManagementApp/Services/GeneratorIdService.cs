using System;
using System.Threading;

namespace PatientManagementApp.Services
{
    public interface IGeneratorIdService
    {
        int GenerateId();
    }

    public class GeneratorIdService : IGeneratorIdService
    {
        private static int currentId = 0;
        private static readonly object lockObj = new object();

        public int GenerateId()
        {
            lock (lockObj)
            {
                return Interlocked.Increment(ref currentId);
            }
        }
    }
}