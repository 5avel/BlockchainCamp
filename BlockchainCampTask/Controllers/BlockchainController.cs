using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainCampTask.Models;
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
            Blockchain.Instance.AddTransaction(transaction);
            return Ok();
        }

        //POST /management/add_link
        /*{	id:int,
        url:string}*/
        [HttpPost("management/add_link")]
        public IActionResult AddLink([FromBody]Neighbour neighbour)
        {
            //Blockchain.Instance.AddData(transaction.from);
            return Ok();
        }

        //GET /management/sync  - вызываем мы что б скачать с соседей начальный блокчейн
        [HttpGet("management/sync")]
        public IActionResult Sync()
        {
            return Ok();
        }

        //GET /management/status
        //{id:string,name:string,last_hash:sha256, neighbours:[‘id1’, ‘id2’,’id3’] }
        [HttpGet("management/status")]
        public Status Status()
        {
            return new Status();
        }

        // GET api/last_blocks/5
        //GET /blockchain/get_blocks/:num_blocks
        [HttpGet("blockchain/get_blocks/{num_blocks}")]
        public IEnumerable<Models.Block> Get(int num_blocks)
        {
            return Blockchain.Instance.GetLastBlocks(num_blocks);
        }
        //POST /blockchain/receive_update
    }
}

