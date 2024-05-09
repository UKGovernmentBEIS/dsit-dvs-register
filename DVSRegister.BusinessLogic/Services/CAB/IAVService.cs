using System;
using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Services.CAB
{
	public interface IAVService
	{
		public GenericResponse ScanFileForVirus(Stream file);
	}
}

