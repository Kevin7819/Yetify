namespace api.Dtos.Activity
{
    public class ActivityDto{
        public int id { set; get; } 
        public int identifyActivity {set; get;}     //identify of the activity
        public string activityName { set; get; }    //name of the activity
        public string activityType { set; get; }    //type of the activity
        public DateTime creationDate { set; get; }  //activity creation date
        public DateTime startDate { set; get; }     //start date of the activity
        public DateTime endDate { set; get; }       //End date of the activity
        public string activityStatus { set; get; }  //Status of the activity
        public string apiSource { set; get; }       //Activity resource API
    }
}