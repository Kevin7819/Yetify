using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Activity{
    public class UpdateActivityRequestDto{
        public DateTime startDate { get; set; }     //start date of the activity
        public DateTime endDate { get; set; }       //End date of the activity
        public string activityStatus { get; set; }  //Status of activity
    }
}