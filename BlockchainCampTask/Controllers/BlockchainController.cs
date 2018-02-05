using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainCampTask.Models;
using BlockchainNode.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainCampTask.Controllers
{
    //[Route("api")]
    public class BlockchainController : Controller
    { 
       
        //POST /management/add_transaction
        [HttpPost("management/add_transaction")]
        public IActionResult AddTransaction([FromBody]Transaction transaction)
        {
            Console.WriteLine("AddTransaction");
            if (transaction == null
                || string.IsNullOrWhiteSpace(transaction.from)
                || string.IsNullOrWhiteSpace(transaction.to)
                || transaction.amount <= 0)
                    return Ok(new Answer(false));

            if(Blockchain.Instance.AddTransaction(transaction))
            {
                return Ok(new Answer(true));
            }
            return Ok(new Answer(false));
        }

        //POST /management/add_link
        /*{	id:int,
        url:string}*/
        [HttpPost("management/add_link")]
        public IActionResult AddLink([FromBody]Neighbour neighbour)
        {
            if(Blockchain.Instance.AddNewNeighbour(neighbour))
            {
                Console.WriteLine("Link added: id - {0}, url - {1}", neighbour.id, neighbour.url);
                return Ok(new Answer(true));
            }
            Console.WriteLine("This link already exists: id - {0}", neighbour.id);
            return Ok(new Answer(false, "Error", "This link already exists."));
        }

        //GET /management/sync  - вызываем мы что б скачать с соседей начальный блокчейн
        [HttpGet("management/sync")]
        public IActionResult Sync()
        {
            Console.WriteLine("Sync");
            if (Blockchain.Instance.Sync())
            {
                return Ok(new Answer(true));
            }
            return Ok(new Answer(false));
        }

        //GET /management/status
        //{id:string,name:string,last_hash:sha256, neighbours:[‘id1’, ‘id2’,’id3’] }
        [HttpGet("management/status")]
        public Status Status()
        {
            Console.WriteLine("GetStatus");
            return Blockchain.Instance.CurStatus;
        }

        [HttpGet("management/state")]
        public Status State()
        {
            Console.WriteLine("GetState");
            return Blockchain.Instance.CurStatus;
        }

        // GET api/last_blocks/5
        //GET /blockchain/get_blocks/:num_blocks
        [HttpGet("blockchain/get_blocks/{num_blocks}")]
        public IEnumerable<Models.Block> GetBlocks(int num_blocks)
        {
            Console.WriteLine("GetBlocks");
            return Blockchain.Instance.GetLastBlocks(num_blocks);
        }
        //POST /blockchain/receive_update

        /*
         *POST /blockchain/receive_update
            {  sender_id:string,  block:{block} }
         */
        [HttpPost("blockchain/receive_update")]
        public IActionResult ReceiveUpdate([FromBody]RUData ruData)
        {
            Console.WriteLine("ReceiveUpdate id-{0}", ruData.sender_id);
            if(Blockchain.Instance.AddNewBlosk(ruData.block, ruData.sender_id))
            {
                return Ok(new Answer(true));
            }
            return Ok(new Answer(false, "ERR_WRONG_HASH", "failed"));
        }

        public class RUData
        {
            public string sender_id { get; set; }
            public Block block { get; set; }
        }
    }
}

