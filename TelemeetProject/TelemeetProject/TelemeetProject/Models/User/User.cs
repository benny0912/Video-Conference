using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public class User
    {

        public partial class Request
        {
            public string imageData { get; set; }
        }

        public partial class Respond
        {
            public bool error { get; set; }
            public string message { get; set; }
        }

        public string user_email { get; set; }
        public string user_password { get; set; }
        public string user_first_name { get; set; }
        public string user_last_name { get; set; }
        public DateTime date_created { get; set; }
        public DateTime signed_in { get; set; }
        public string last_room { get; set; }
        public string user_role { get; set; }
        public DateTime image_created { get; set; }
        public string user_image { get; set; }

    }
}
