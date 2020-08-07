using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models.Room
{
    public interface IRoomDB
    {
        bool isRoomExists(string roomName);
        string GetRoom(string roomId);
        bool validateRoomWithPass(string roomName, string roomPass);
        void createRoom(RoomModel room);
        void deleteRoom(string userEmail);
        bool isUserCreatedRoom(string userEmail);
        string GetRoomId(string roomName);
        string getRoomPassword(string roomId);

    }
}
