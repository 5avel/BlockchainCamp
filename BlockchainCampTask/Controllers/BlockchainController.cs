using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainCampTask.Controllers
{
    [Route("api")]
    public class BlockchainController : Controller
    {

        // GET api/last_blocks/5
        [HttpGet("last_blocks/{count}")]
        public IEnumerable<Models.Block> Get(int count)
        {
            return Blockchain.Instance.GetLastBlocks(count);
        }

        // POST api/add_data/data:'{data}'
        [HttpPost("add_data/data:{data}")]
        public IActionResult Post(string data)
        {
            Blockchain.Instance.AddData(data);
           
            return Ok();
        }

        // POST api/add_data   JsonFromBody
        [HttpPost("add_data")]
        public IActionResult PostBody([FromBody]Data data)
        {
            Blockchain.Instance.AddData(data.Value);
            return Ok();
        }

    }

    public struct Data
    {
        public string Value { get; set; }
    }
}
