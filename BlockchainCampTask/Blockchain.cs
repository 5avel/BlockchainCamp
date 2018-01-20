using BlockchainCampTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainCampTask
{
    public class Blockchain
    {
        public Dictionary<string, Block> Blocks { get; private set; }

        public Block LastBlock { get; private set; }

        private List<string> _listData = new List<string>();

        public void AddData(string data)
        {
            _listData.Add(data);
            if(_listData.Count == Block.RowsCount)
            {
                CreateNewBlock();
            }
        }

        public IEnumerable<Block> GetLastBlocks(int blocksCount = 0)
        {
            List<Block> blocks = new List<Block>();
            if (blocksCount == 0)
            {
               // return Blocks.ToList<Block>();
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
                   LastBlock != null ? LastBlock.block_hash : "0",
                   _listData.ToArray<string>());
            Blocks.Add(block.block_hash, block);
            LastBlock = block;
            _listData.Clear();
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
