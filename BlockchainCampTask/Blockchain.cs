using BlockchainCampTask.Models;
using BlockchainNode.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainCampTask
{
    public class Blockchain
    {
        private BlockchainRepository blockchainRepository;

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

        private Status _curStatus;
        public Status CurStatus
        {
            get
            {
                if(_curStatus == null)
                {
                    _curStatus = blockchainRepository.GetStatus();
                }
                return _curStatus;
            }
        }

        private List<Neighbour> _links;
        public List<Neighbour> Links
        {
            get
            {
                if (_links == null)
                {
                    _links = blockchainRepository.GetAllNeighbours().ToList();
                }
                return _links;
            }
        }

        internal bool Sync()
        {
            Neighbour neighbour = blockchainRepository.GetFirstNeighbour();
            using (WebClient client = new WebClient())
            {
                string json =  client.DownloadString(neighbour.url + "/blockchain/get_blocks/10000");
                List<Block> listBlocks = JsonConvert.DeserializeObject<List<Block>>(json);

                if(listBlocks.Count() == 0) return false;

                blockchainRepository.DelAllBlock();
                foreach (Block item in listBlocks)
                {
                    blockchainRepository.AddNewBlock(item);
                    CurStatus.last_hash = item.hash;
                }

                UpdateStatus();
                //Console.WriteLine(json);
                return true;
            }
        }

        internal bool AddNewNeighbour(Neighbour neighbour)
        {
            foreach (string n in CurStatus.neighbours)
            {
                if (neighbour.id == n) return false;
            }
            CurStatus.neighbours.Add(neighbour.id);
            Links.Add(neighbour);
            blockchainRepository.AddNewNeighbour(neighbour);
            UpdateStatus();
            return true;
        }

        private void UpdateStatus()
        {
            blockchainRepository.UpdateStatus(_curStatus);
        }

        public IEnumerable<Block> GetLastBlocks(int blocksCount = 0)
        {
            List<Block> blocks = new List<Block>();
            if (CurStatus.last_hash == "0") return blocks; // блоков нет

            int counter = 0;

            Block curBlock = blockchainRepository.GetBlockByHash(CurStatus.last_hash);
            blocks.Add(curBlock); // последний блок
            while(curBlock.prev_hash != "0" )
            {
                
                curBlock = blockchainRepository.GetBlockByHash(curBlock.prev_hash);
                blocks.Add(curBlock);
                counter++;
                if (blocksCount > 0 && counter == blocksCount) break;
            }
            blocks.Reverse();
            return blocks;
        }

        public bool AddNewBlosk(Block block, string senderId)
        {
            if (block.prev_hash == _curStatus.last_hash)
            {
                blockchainRepository.AddNewBlock(block);
                CurStatus.last_hash = block.hash;
                UpdateStatus();

                SendBlockToNeighbour(block, senderId); // отправить соседям
                return true;
            }
            else if(block.hash == _curStatus.last_hash)
            {
                Block myLastBlock = blockchainRepository.GetBlockByHash(_curStatus.last_hash);
                if (block.prev_hash == myLastBlock.prev_hash)
                {
                    if (block.ts < myLastBlock.ts)
                    {
                        blockchainRepository.DelBlockByHash(_curStatus.last_hash); // del my last block
                        blockchainRepository.AddNewBlock(block);
                        CurStatus.last_hash = block.hash;
                        UpdateStatus();
                        SendBlockToNeighbour(block, senderId); // отправить соседям
                        return true;
                    }
                }
            }
            return false;
        }

        private void SendBlockToNeighbour(Block blockToSend, string senderId = "")
        {
            List<Neighbour> neighbours = blockchainRepository.GetAllNeighbours().ToList();
            if (neighbours.Count() == 0) return;

            foreach (var item in neighbours)
            {

                if (item.id == senderId) continue; // пропуск сендера

                using (var client = new HttpClient())
                {
                    client.PostAsync(
                        item.url + "/blockchain/receive_update",
                        new JsonContent(
                            new { sender_id = CurStatus.id, block = blockToSend }
                            ));
                }
            }
        }


        private void CreateNewBlock()
        {
            Block block = new Block(CurStatus.last_hash, _listTransaction);

            blockchainRepository.AddNewBlock(block); 

            CurStatus.last_hash = block.hash;
            UpdateStatus();

            _listTransaction.Clear();

            SendBlockToNeighbour(block);
        }

        private Blockchain()
        {
            blockchainRepository = new BlockchainRepository();
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





    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }
}
