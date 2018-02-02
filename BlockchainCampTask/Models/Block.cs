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
        public Block(string prev_hash, List<Transaction> tx)
        {
            if (tx.Count != RowsCount || String.IsNullOrEmpty(prev_hash))
                throw new ArgumentException();

            this.prev_hash = prev_hash;
            this.tx = tx;
            this.ts = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            this.hash = GetHash();
        }
        public static readonly int RowsCount = 5;
        public string prev_hash { get; }
        public List<Transaction> tx { get; }

        //[JsonConverter(typeof(FormatConverter))]
        public Int64 ts { get; }
        [LiteDB.BsonId]
        public string hash {get;}

        public string GetHash()
        {
            string block_data = this.prev_hash;
           
            foreach (Transaction trans in tx)
            {
                block_data += trans.from;
                block_data += trans.to;
                block_data += trans.amount;
            }

            block_data += ts.ToString();
            string res;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(block_data));
                res = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            return res;
        }

    }

    public class FormatConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Int64))
            {
                return Convert.ToInt64(reader.Value.ToString().Replace(".", string.Empty));
            }

            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }
    }
}
