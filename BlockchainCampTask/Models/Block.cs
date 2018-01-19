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
        public Block(string previous_block_hash, string[] rows, int timestamp)
        {
            this.previous_block_hash = previous_block_hash;
            if (rows.Length != 5) throw new ArgumentException();
            this.rows = rows;
            this.timestamp = timestamp;
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                this.block_hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

        }
        public string previous_block_hash { get; }
        public string[] rows { get; }
        //int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        public int timestamp { get; }
        public string block_hash { get; }

    }
}
