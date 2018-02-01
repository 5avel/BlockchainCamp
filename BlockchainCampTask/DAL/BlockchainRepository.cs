using BlockchainCampTask.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BlockchainNode.DAL
{
    public class BlockchainRepository
    {
        // get Blocks (0  = all blocks, count < num - all blocks)
        public List<Block> GetBlocks(int count)
        {
            List<Block> blocksToReturn = new List<Block>();
            using (var db = new LiteDatabase(@"Data/Blockchain.db"))
            {
                // Get customer collection
                var blocks = db.GetCollection<Block>("blocks");
                var results = blocks.FindAll();
                foreach (Block block in results)
                {
                    blocksToReturn.Add(block);
                }
                return blocksToReturn;
            }
        }
        // add new block

        // get all links
        // add link

        // get all Transactions
        // add Transaction

        // GetStatus
        // save status
    }
}
