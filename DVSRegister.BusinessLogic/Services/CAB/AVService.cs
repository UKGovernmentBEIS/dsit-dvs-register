using System;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
	public class AVService : IAVService
	{
		public AVService()
		{
		}

        public GenericResponse ScanFileForVirus(Stream file)
        {
            GenericResponse genericResponse = new GenericResponse();

            genericResponse.Success = true;

            return genericResponse;
        }
    }
}

