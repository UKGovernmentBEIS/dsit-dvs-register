using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.BusinessLogic.Models.Register
{
    public class RegisterPublishLogDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
