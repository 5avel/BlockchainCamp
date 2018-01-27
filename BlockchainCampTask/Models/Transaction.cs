using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public int amount { get; set; }
    }
}
