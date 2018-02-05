using System.Collections.Generic;

namespace BlockchainCampTask.Models
{
    public class Status
    {
        public string id { get; }
        public string name { get; }
        public string last_hash { set; get; }
        public List<Neighbour> neighbours { get; set; }

        public string url { get; set; }

        public Status()
        {
            this.id = "66";
            this.name = "Pavel_66";
        }
    }
}
