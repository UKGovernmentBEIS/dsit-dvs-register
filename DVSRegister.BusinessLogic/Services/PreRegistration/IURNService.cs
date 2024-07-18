using DVSRegister.BusinessLogic.Models.PreRegistration;

namespace DVSRegister.BusinessLogic.Services.PreRegistration
{
	public interface IURNService
	{
        /// <summary>
        /// Block 1
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public string CreateBlock1(string CompanyName);

        /// <summary>
        /// Block 2
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public string CreateBlock2(DateTime Date);

        /// <summary>
        /// Block 3
        /// </summary>
        /// <param name="FullName"></param>
        /// <returns></returns>
        public string CreateBlock3(string FullName);

        /// <summary>
        /// Block 4
        /// </summary>
        /// <returns></returns>
        public string CreateBlock4();

        /// <summary>
        /// CheckSum
        /// </summary>
        /// <returns></returns>
        public byte CalculateChecksum(string block1, string block2, string block3, string block4);


        /// <summary>
        /// Generates URN and returns response
        /// </summary>
        /// <param name="preRegistrationDto"></param>
        /// <returns></returns>
        public URNDto GenerateURN(PreRegistrationDto preRegistrationDto);
	}
}