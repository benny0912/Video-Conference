using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public class RoomModel
    {
        public class Request : CommonGetModels
        {
            public string room_name { get; set; }
            public string room_password { get; set; }
        }
        public string room_id { get; set; }
        public string room_name { get; set; }
        public string room_password { get; set; }
        public string user_email { get; set; }
    }
}
