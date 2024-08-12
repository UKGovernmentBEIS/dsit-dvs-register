using DVSRegister.CommonUtility.Models;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class QualityLevelDto
    {      
        public int Id { get; set; }
        public string Level { get; set; }
        public QualityTypeEnum QualityType { get; set; }
    }
}
