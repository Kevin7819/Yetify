namespace api.Models
{
    public class Activity
    {
        public int id { get; set; }
        public int identifyActivity {set; get;}
        public string activityName { get; set; } //name of activity
        public string activityType { get; set; } //type of activity
        public DateTime creationDate { get; set; } //activity creation date
        public DateTime startDate { get; set; } //start date of the activity
        public DateTime endDate { get; set; } //End date of the activity
        public string activityStatus { get; set; } //Status of activity
        public string apiSource { get; set; } //Activity resource API
    }
}
