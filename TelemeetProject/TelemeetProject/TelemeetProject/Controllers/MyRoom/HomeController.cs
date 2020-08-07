using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelemeetProject.Models;
using TelemeetProject.Models.Room;

namespace TelemeetProject.Controllers.MyRoom
{
    public class RoomController : Controller
    {
        private readonly IRoomUserDB _roomUserDB;
        private readonly IUserDB _userDB;
        private readonly IRoomDB _roomDB;
        private readonly IActivityDB _activityDB;

        public RoomController(IRoomUserDB roomUserDB, IUserDB userDB, IRoomDB roomDB, IActivityDB activityDB)
        {
            _roomUserDB = roomUserDB;
            _userDB = userDB;
            _roomDB = roomDB;
            _activityDB = activityDB;
        }

        public IActionResult Index()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("userEmail")))
            {
                TempData["msg"] = "Please login to proceed.";
                return RedirectToAction("Index", "Login");
            } else
            {
                if(string.IsNullOrEmpty(HttpContext.Session.GetString("roomId")))
                {
                    TempData["msg"] = "Invalid room id.";
                    return RedirectToAction("Index", "Main");
                } else
                {
                    var roomid = HttpContext.Session.GetString("roomId");
                    var users = _roomUserDB.getUsersInRoom(roomid);
                    var userList = new List<User>();
                    foreach (string temp in users)
                    {
                        userList.Add(_userDB.getUser(temp));
                    }
                    return View("~/Views/MyRoom/MyRoom.cshtml", userList);
                }
            }
        }

        [HttpPost]
        public ActionResult WebPageClose()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("roomId")))
            {
                var userEmail = HttpContext.Session.GetString("userEmail");
                var roomid = HttpContext.Session.GetString("roomId");
                _roomUserDB.deleteRoomUser(userEmail);
                if (_roomUserDB.getUsersInRoom(roomid).Count == 0)
                {
                    _roomDB.deleteRoom(roomid);
                }
                Activity roomActivity = new Activity
                {
                    user_email = HttpContext.Session.GetString("userEmail"),
                    user_first_name = HttpContext.Session.GetString("firstName"),
                    user_last_name = HttpContext.Session.GetString("lastName"),
                    activity_type = "Leave room",
                    activity_details = HttpContext.Session.GetString("roomName"),
                    last_activity = DateTime.Now,
                };
                _activityDB.logActivity(roomActivity);
                HttpContext.Session.Remove("roomId");
                HttpContext.Session.Remove("roomName");
            }
            return Ok();
        }

        public IActionResult JoinRoom(string roomId, string roomPass)
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("userEmail")))
            {
                TempData["msg"] = "Please login to proceed.";
                return RedirectToAction("Index", "Login");
            } else
            {
                var roomName = _roomDB.GetRoom(roomId);
                if (!string.IsNullOrEmpty(roomName))
                {
                    if(roomPass.Equals(_roomDB.getRoomPassword(roomId)))
                    {
                        HttpContext.Session.SetString("roomId", roomId);
                        HttpContext.Session.SetString("roomName", roomName);
                        HttpContext.Session.SetString("roomPass", _roomDB.getRoomPassword(roomId));
                        _userDB.updateUserLastRoom(HttpContext.Session.GetString("userEmail"), roomName);
                        _roomUserDB.insertRoomUsers(roomId, HttpContext.Session.GetString("userEmail"));
                        logActivity("Join room", roomName);
                        return RedirectToAction("Index", "Room");
                    }
                    else
                    {
                        TempData["msg"] = "Invalid room password.";
                        return RedirectToAction("Index", "Main");
                    }
                }
                else
                {
                    TempData["msg"] = "Invalid room id.";
                    return RedirectToAction("Index", "Main");
                }
            }
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var roomid = HttpContext.Session.GetString("roomId");
            var userList = new List<User>();
            if (!string.IsNullOrEmpty(roomid))
            {
                var users = _roomUserDB.getUsersInRoom(roomid);
                foreach (string temp in users)
                {
                    userList.Add(_userDB.getUser(temp));
                }
            }
            return Ok(userList);
        }

        [HttpPost]
        public ActionResult LeaveRoom()
        {
            var roomid = HttpContext.Session.GetString("roomId");
            var userEmail = HttpContext.Session.GetString("userEmail");
            _roomUserDB.deleteRoomUser(userEmail);
            if(_roomUserDB.getUsersInRoom(roomid).Count == 0)
            {
                _roomDB.deleteRoom(roomid);
            }
            Activity activity = new Activity
            {
                user_email = HttpContext.Session.GetString("userEmail"),
                user_first_name = HttpContext.Session.GetString("firstName"),
                user_last_name = HttpContext.Session.GetString("lastName"),
                activity_type = "Leave room",
                activity_details = HttpContext.Session.GetString("roomName"),
                last_activity = DateTime.Now,
            };
            _activityDB.logActivity(activity);
            HttpContext.Session.Remove("roomId");
            HttpContext.Session.Remove("roomName");
            return RedirectToAction("Index", "Main");
        }

        private void logActivity(string activityType, string activity_details)
        {
            Activity activity = new Activity
            {
                user_email = HttpContext.Session.GetString("userEmail"),
                user_first_name = HttpContext.Session.GetString("firstName"),
                user_last_name = HttpContext.Session.GetString("lastName"),
                activity_type = activityType,
                activity_details = activity_details,
                last_activity = DateTime.Now,
            };
            _activityDB.logActivity(activity);
        }
    }
}
