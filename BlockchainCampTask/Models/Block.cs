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
        public Block(string previous_block_hash, string[] rows)
        {
            if (rows.Length != RowsCount || String.IsNullOrEmpty(previous_block_hash))
                throw new ArgumentException();

            this.previous_block_hash = previous_block_hash;
            this.rows = rows;
            this.timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            this.block_hash = GetHash();
        }
        public static readonly int RowsCount = 5;
        public string previous_block_hash { get; }
        public string[] rows { get; }
        public int timestamp { get; }
        public string block_hash { get; }

        public string GetHash()
        {
            string data = this.previous_block_hash;

            foreach (string row in rows)
                data += row;

            data += timestamp.ToString();
            string res;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                res = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            return res;
        }

    }
}
