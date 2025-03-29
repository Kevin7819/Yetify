using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Activity{
    public class CreateActivityRequestDto{
        public string activityName { get; set; }    //name of activity
        public int identifyActivity {set; get;}
        public string activityType { get; set; }    //type of activity
        public DateTime creationDate { get; set; }  //activity creation date
        public DateTime startDate { get; set; }     //start date of the activity
        public DateTime endDate { get; set; }       //End date of the activity
        public string activityStatus { get; set; }  //Status of activity
        public string apiSource { get; set; }       //Activity resource API
    }
}