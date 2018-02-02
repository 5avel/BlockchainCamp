using BlockchainCampTask.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace BlockchainNode.DAL
{
    public class BlockchainRepository
    {

        internal readonly string dataPath = @"Data/Blockchain.db";
      

        public Block GetBlockByHash(string hash)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var blocks = db.GetCollection<Block>("blocks");
                return blocks.FindOne(x => x.hash == hash);
            }
        }

        public void DelBlockByHash(string hash)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var blocks = db.GetCollection<Block>("blocks");
                blocks.Delete(x => x.hash == hash);
            }
        }

        public void DelAllBlock()
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var blocks = db.GetCollection<Block>("blocks");
                blocks.Delete(x => x.hash != "");
            }
        }

        public void AddNewBlock(Block block)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var blocks = db.GetCollection<Block>("blocks");
                blocks.Insert(block);
            }
        }

        public IEnumerable<Neighbour> GetAllNeighbours()
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var neighbours = db.GetCollection<Neighbour>("neighbours");
                return neighbours.FindAll();
            }
        }

        public Neighbour GetFirstNeighbour()
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var neighbours = db.GetCollection<Neighbour>("neighbours");
                return neighbours.FindOne(x => !String.IsNullOrWhiteSpace(x.url));
            }
        }

        public void AddNewNeighbour(Neighbour neighbour)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var neighbours = db.GetCollection<Neighbour>("neighbours");
                neighbours.Insert(neighbour);
            }
        }

        public IEnumerable<Transaction> GetAllTransaction()
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var transactions = db.GetCollection<Transaction>("transactions");
                return transactions.FindAll();
            }
        }

        public void AddNewTransaction(Transaction transaction)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var transactions = db.GetCollection<Transaction>("neighbours");
                transactions.Insert(transaction);
            }
        }

        public Status GetStatus()
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var status = db.GetCollection<Status>("status");
                if (status.Count() == 0)
                    status.Insert(new Status { last_hash = "0", neighbours = new List<string>(), url = "192.168.88.27:8770" });
                return status.FindOne(x => !String.IsNullOrWhiteSpace(x.name));
            }
        }

        public void UpdateStatus(Status status)
        {
            using (var db = new LiteDatabase(dataPath))
            {
                var statusdb = db.GetCollection<Status>("status");
                statusdb.Delete(x => !String.IsNullOrWhiteSpace(x.name));
                statusdb.Insert(status);
            }
        }
    }
}
