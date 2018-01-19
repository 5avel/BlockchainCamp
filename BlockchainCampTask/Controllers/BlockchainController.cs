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
        public string Get(int count)
        {
            return "value";
        }

        // POST api/add_data/data:'{data}'
        [HttpPost("add_data/data:{data}")]
        public string Post(string data)
        {
            return data;
        }

        // POST api/add_data
        [HttpPost("add_data")]
        public string PostBody([FromBody]Data data)
        {
            return data.Value;
        }

    }

    public struct Data
    {
        public string Value { get; set; }

    }
}
