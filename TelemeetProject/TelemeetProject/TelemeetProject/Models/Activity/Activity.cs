using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public class Activity
    {
        public string user_email { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public string activity_type { get; set; }
        public string activity_details { get; set; }
        public DateTime last_activity { get; set; }
        public int activity_log_id { get; set; }
 
    }
}
