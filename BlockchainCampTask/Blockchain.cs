using BlockchainCampTask.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainCampTask
{
    public class Blockchain
    {

        private object syncRoot = new object();
        private List<Transaction> _listTransaction = new List<Transaction>();

        public void AddTransaction(Transaction transaction)
        {
            _listTransaction.Add(transaction);
            if(_listTransaction.Count == Block.RowsCount)
            {
                CreateNewBlock();
            }
        }

        private Status _status;
        public Status CurStatus
        {
            get
            {
                if(_status == null)
                {
                    Status s;
                    lock (syncRoot)
                    {
                        using (StreamReader file = File.OpenText(String.Format("./blocks/{0}.json", "Status")))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            s = (Status)serializer.Deserialize(file, typeof(Status));
                        }
                    }
                    _status = s;
                }
                return _status;
            }
            set
            {
                using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Status")))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                _status = value;
            }
        }

        private List<Link> _links;
        public List<Link> Links
        {
            get
            {
                if (_links == null)
                {
                    List<Link> l;
                    lock (syncRoot)
                    {
                        using (StreamReader file = File.OpenText(String.Format("./blocks/{0}.json", "Links")))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            l = (List<Link>)serializer.Deserialize(file, typeof(List<Link>));
                        }
                    }
                    _links = l;
                }
                return _links;
            }
            set
            {
                using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Links")))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, value);
                }
                _links = value;
            }
        }

        public void UpdateStatus()
        {
            using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Status")))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, CurStatus);
            }

            using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", "Links")))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, Links);
            }

        }

        public IEnumerable<Block> GetLastBlocks(int blocksCount = 0)
        {
            List<Block> blocks = new List<Block>();
            if (CurStatus.last_hash == "0") return blocks;

            Block lastBlock = GetBlockByHash(CurStatus.last_hash);
            Block curBlock = lastBlock;
            blocks.Add(lastBlock); // первый блок
            while(curBlock.prev_hash != "0")
            {
                curBlock = GetBlockByHash(curBlock.prev_hash);
                blocks.Add(curBlock);
            }
            return blocks;
        }

        private Block GetBlockByHash(string hash)
        {
            Block b;
            using (StreamReader file = File.OpenText(String.Format("./blocks/{0}.json", hash)))
            {
                JsonSerializer serializer = new JsonSerializer();
                b = (Block)serializer.Deserialize(file, typeof(Block));
            }
            return b;
        }

        public void AddNewBlosk(Block block)
        {
            using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", block.hash)))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, block);
            }

            CurStatus.last_hash = block.hash;
            UpdateStatus();
        }


        private void CreateNewBlock()
        {
            Block block = new Block(CurStatus.last_hash, _listTransaction);

            using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", block.hash)))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, block);
            }

            CurStatus.last_hash = block.hash;
            UpdateStatus();

            _listTransaction.Clear();
        }

        private Blockchain()
        {
          
        }
        private static Blockchain instance;
        public static Blockchain Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Blockchain();
                }
                return instance;
            }
        }
    }
}
