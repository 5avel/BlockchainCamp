namespace BlockchainNode.Models
{
    //{"success":true,"status":"OK","message":"success"}
    public class Answer
    {
        public bool success { set; get; }
        public string status { set; get; }
        public string message { set; get; }

        public Answer(){   }
        public Answer(bool success, string status, string message)
        {
            this.success = success;
            this.status = status;
            this.message = message;
        }
        public Answer(bool success)
        {
            this.success = success;
            if (success)
            {
                this.status = "OK";
                this.message = "success";
            }
            else
            {
                this.status = "Error";
                this.message = "failed";
            }
        }
    }
}
