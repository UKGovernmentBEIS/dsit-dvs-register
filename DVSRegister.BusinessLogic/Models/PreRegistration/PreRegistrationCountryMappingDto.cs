using DVSRegister.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.BusinessLogic.Models.PreRegistration
{
    public class PreRegistrationCountryMappingDto
    {      
        public int PreRegistrationId { get; set; }
        public PreRegistrationDto PreRegistration { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
