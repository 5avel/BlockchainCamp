using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlockchainCampTask.Models
{
         /* {	from:string,
                to:string,
                amount:int
            }*/
    public struct Transaction
    {
        public string from { get; set; }
        public string to { get; set; }

        [JsonConverter(typeof(FormatConverter))]
        public Int64 amount { get; set; }
    }
}
