﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainCampTask.Models
{
    //{id:string,name:string,last_hash:sha256, neighbours:[‘id1’, ‘id2’,’id3’] }
    public class Status
    {
        public string id { get; }
        public string name { get; }
        public string last_hash { set; get; }
        public List<string> neighbours { get; set; }

        public string url { get; set; }

        public Status()
        {
            this.id = "66";
            this.name = "Pavel_66";
        }
    }
}
