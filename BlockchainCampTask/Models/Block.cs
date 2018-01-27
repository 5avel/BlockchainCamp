using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainCampTask.Models
{
    public class Block
    {
        public Block(string previous_block_hash, List<Transaction> transactions)
        {
            if (transactions.Count != RowsCount || String.IsNullOrEmpty(previous_block_hash))
                throw new ArgumentException();

            this.previous_block_hash = previous_block_hash;
            this.transactions = transactions;
            this.timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            this.block_hash = GetHash();
        }
        public static readonly int RowsCount = 5;
        public string previous_block_hash { get; }
        public List<Transaction> transactions { get; }
        public int timestamp { get; }
        public string block_hash {get;}

        public string GetHash()
        {
            string block_data = this.previous_block_hash;
           
            foreach (Transaction trans in transactions)
            {
                block_data += JsonConvert.SerializeObject(trans);
            }

            block_data += timestamp.ToString();
            string res;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(block_data));
                res = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            return res;
        }

    }
}
