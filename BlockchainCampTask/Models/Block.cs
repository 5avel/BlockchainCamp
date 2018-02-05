using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainCampTask.Models
{
    public class Block
    {
        public static readonly int RowsCount = 5;
        public string prev_hash { get; set; }
        public List<Transaction> tx { get; set; }
        public Int64 ts { get; set; }
        [LiteDB.BsonId]
        public string hash { get; set; }


        public Block() {  }
        public Block(string prev_hash, List<Transaction> tx)
        {
            if (tx.Count != RowsCount || String.IsNullOrWhiteSpace(prev_hash))
                throw new ArgumentException();

            this.prev_hash = prev_hash;
            this.tx = new List<Transaction>(tx);// новый лист
            this.ts = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            this.hash = GetHash();
        }

        public string GetHash()
        {
            //sha256(prev_hash + ts.toString()
            //+ tx[0].from + tx[0].to+tx[0].amount.toString() + tx[1].from + tx[1].to + tx[1].amount.toString() … )
            string block_data = this.prev_hash;
            block_data += ts.ToString();
            foreach (Transaction trans in tx)
            {
                block_data += trans.from;
                block_data += trans.to;
                block_data += trans.amount;
            }
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(block_data));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
