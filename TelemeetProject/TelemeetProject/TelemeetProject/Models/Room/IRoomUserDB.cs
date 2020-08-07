using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models.Room
{
    public interface IRoomUserDB
    {
        List<string> getUsersInRoom(string roomid);
        void insertRoomUsers(string roomid, string userEmail);
        void deleteRoomUser(string userEmail);
    }
}
