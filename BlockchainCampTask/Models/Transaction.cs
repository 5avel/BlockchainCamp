using System;

namespace BlockchainCampTask.Models
{
    public class Transaction
    {
        public string from { get; set; }
        public string to { get; set; }
        public Int64 amount { get; set; }
    }
}
