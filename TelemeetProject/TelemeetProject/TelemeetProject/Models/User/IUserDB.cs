using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelemeetProject.Models
{
    public interface IUserDB
    {
        User.Respond loginUser(User user);
        User getUser(User user);
        User.Respond createUser(User user);
        User getUser(string userEmail);
        bool checkUserWithPassword(User user);
        bool checkUserExist(User user);
        void updateUser(User user);
        void updateUserWithEmail(User user, string oldEmail);
        void updateUserSignIn(string userEmail);
        void updateUserLastRoom(string userEmail, string roomName);
        void updateUserImg(string userEmail, string imageUrl);
        List<User> getUsers();
    }
}
