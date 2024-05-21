using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
	public interface IBucketService
	{
		public Task<GenericResponse> WriteToS3Bucket(Stream file, string fileName);
	}
}

