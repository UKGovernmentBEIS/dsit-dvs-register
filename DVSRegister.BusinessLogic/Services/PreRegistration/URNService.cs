using System.Security.Cryptography;
using DVSRegister.BusinessLogic.Models.PreRegistration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DVSRegister.BusinessLogic.Services.PreRegistration
{
	public class URNService : IURNService
	{
        public string CreateBlock1(string CompanyName)
        {
            CompanyName = CompanyName.Replace(" ", "");
            
            if(CompanyName.Length < 5)
            {
                string subsetString = "6789";
                int remainingLength = 5 - CompanyName.Length;
                return string.Concat(CompanyName, subsetString.AsSpan(subsetString.Length - remainingLength));
            }

            return CompanyName[..5];
        }

        public string CreateBlock2(DateTime Date)
        {
            DateTime utcDate = Date.ToUniversalTime();
            long secondsSinceUnixEpoch = (long)(utcDate - new DateTime(1970, 1, 1)).TotalSeconds;
            return secondsSinceUnixEpoch.ToString();
        }

        public string CreateBlock3(string FullName)
        {
            string[] nameParts = FullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string initials = "";
            foreach (string namePart in nameParts)
            {
                initials += char.ToUpper(namePart[0]); // Ensure the initial is uppercase
            }
            return initials;
        }

        public string CreateBlock4()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var numberBytes = new byte[2];
                rng.GetBytes(numberBytes);
                int number = BitConverter.ToUInt16(numberBytes, 0) % 1000;
                return number.ToString("D3");
            }
        }

        public byte CalculateChecksum(string block1, string block2, string block3, string block4)
        {
            // Placeholder for the actual checksum calculation
            // This should be replaced by a method provided by the tech team
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(block1+block2+block3+block4);
            byte[] hash = md5.ComputeHash(inputBytes);
            return hash[0];
        }

        public URNDto GenerateURN(PreRegistrationDto preRegistrationDto)
        {
            string block1 = CreateBlock1(preRegistrationDto.RegisteredCompanyName);
            string block2 = CreateBlock2(Convert.ToDateTime(preRegistrationDto.CreatedDate));
            string block3 = CreateBlock3(preRegistrationDto.FullName);
            string block4 = CreateBlock4();

            byte checksum = CalculateChecksum(block1, block2, block3, block4);

            URNDto CreatedURN = new URNDto() { URN = $"{block1}-{block2}-{block3}-{block4}-{checksum}" };

            return CreatedURN;
        }
    }
}
