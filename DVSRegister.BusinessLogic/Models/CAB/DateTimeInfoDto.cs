using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVSRegister.BusinessLogic.Models.CAB
{
    public class DateTimeInfoDto
    {
        public DateTime LastModifiedTime { get; set; }
        public long EpochTime => new DateTimeOffset(LastModifiedTime).ToUnixTimeSeconds();

    }
}
