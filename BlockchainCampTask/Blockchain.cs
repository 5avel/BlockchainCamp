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
        public Dictionary<string, Block> Blocks { get; private set; }

        public Block LastBlock { get; private set; }

        private List<Transaction> _listTransaction = new List<Transaction>();

        private string fileToRead = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));

        public void AddTransaction(Transaction transaction)
        {
            _listTransaction.Add(transaction);
            if(_listTransaction.Count == Block.RowsCount)
            {
                CreateNewBlock();
            }
        }

        public IEnumerable<Block> GetLastBlocks(int blocksCount = 0)
        {
            List<Block> blocks = new List<Block>();
            if (blocksCount > Blocks.Count) blocksCount = Blocks.Count;
            if (blocksCount <= 0)
            {
                return null;
            }
            else
            {
                blocks.Add(LastBlock);
                for (int i = 1; i < blocksCount; i++)
                {
                    blocks.Add(Blocks[blocks[i - 1].previous_block_hash]);
                }
            }
            return blocks;
        }

        private void CreateNewBlock()
        {
            Block block = new Block(
                   LastBlock != null ? LastBlock.block_hash : "0", _listTransaction);

            using (StreamWriter file = File.CreateText(String.Format("./blocks/{0}.json", block.block_hash)))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, block);
            }
            Blocks.Add(block.block_hash, block);
            LastBlock = block;
            _listTransaction.Clear();
        }

        private Blockchain()
        {
            Blocks = new Dictionary<string, Block>();
            LastBlock = null;
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
