using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using TelemeetProject.Hubs;
using TelemeetProject.Models;
using TelemeetProject.Models.Room;

namespace TelemeetProject.Controllers.Home
{
    public class MainController : Controller
    {

        private readonly IRoomUserDB _roomUserDB;
        private readonly IRoomDB _roomDB;
        private readonly IUserDB _userDB;
        private readonly IActivityDB _activityDB;
        private readonly IHubContext<RoomHub> context;
        private readonly ITimeDB _timeDB;

        public MainController(IRoomDB roomDB, IRoomUserDB roomUserDB, IUserDB userDB, IActivityDB activityDB, IHubContext<RoomHub> hubContext, ITimeDB timeDB)
        {
            _roomDB = roomDB;
            _roomUserDB = roomUserDB;
            _userDB = userDB;
            _activityDB = activityDB;
            context = hubContext;
            _timeDB = timeDB;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("userEmail")))
            {
                TempData["msg"] = "Please login to proceed.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
                
        }

        public IActionResult InsertImage(Models.User.Request request)
        {
            var userEmail = HttpContext.Session.GetString("userEmail");
            if (!string.IsNullOrEmpty(request.imageData))
            {
                _userDB.updateUserImg(userEmail, request.imageData);
                context.Clients.All.SendAsync("refreshUsers");
            }
            return Ok();
        }

       [HttpPost]
        public IActionResult CreateRoom([Bind] RoomModel obj)
        {
            string roomName = obj.room_name.ToString();
            if (_roomDB.isRoomExists(roomName))
            {
                TempData["msg"] = "Room Name Existed.";
                return View("~/Views/Main/Index.cshtml");
            }
            else
            {
                obj.room_id = Guid.NewGuid().ToString("N").Substring(0, 12); //auto generate id
                obj.user_email = HttpContext.Session.GetString("userEmail");
                _roomDB.createRoom(obj);
                _roomUserDB.insertRoomUsers(obj.room_id, obj.user_email);
                HttpContext.Session.SetString("roomId", obj.room_id);
                HttpContext.Session.SetString("roomName", obj.room_name);
                HttpContext.Session.SetString("roomPass", _roomDB.getRoomPassword(obj.room_id));
                logActivity("Create room", obj.room_name);
                return RedirectToAction("Index", "Room");
            }
        }

        //JoinRooom-------------------------------------------------------------
        [HttpPost]
        public IActionResult JoinRoom([Bind] RoomModel obj)
        {
            //check room id
            if (!_roomDB.isRoomExists(obj.room_name))
            {
                TempData["msg"] = "Room does not exists.";
                return View("~/Views/Main/Index.cshtml");
            }
            else
            {
                //check password
                if (_roomDB.validateRoomWithPass(obj.room_name, obj.room_password)) //if same pass
                {
                    var roomId = _roomDB.GetRoomId(obj.room_name);
                    _userDB.updateUserLastRoom(HttpContext.Session.GetString("userEmail"), obj.room_name);
                    _roomUserDB.insertRoomUsers(roomId, HttpContext.Session.GetString("userEmail"));
                    HttpContext.Session.SetString("roomId", roomId);
                    HttpContext.Session.SetString("roomName", obj.room_name);
                    HttpContext.Session.SetString("roomPass", _roomDB.getRoomPassword(obj.room_id));
                    logActivity("Join room", obj.room_name);
                    return RedirectToAction("Index", "Room");
                }
                else
                {
                    TempData["msg"] = "Invalid Pass.";
                    return View("~/Views/Main/Index.cshtml");
                }
            }
        }

        [HttpPost]
        public IActionResult UpdateProfile(User user)
        {
            var msg = "Successfully updated";
            var status = true;
            var userEmail = HttpContext.Session.GetString("userEmail");
            if (user.user_email.Equals(userEmail))
            {
                _userDB.updateUser(user);
                logActivity("Update profile", "Email = " + userEmail + ", First name = " + user.user_first_name + ", Last name = " + user.user_last_name);
            }
            else
            {
                if (!_userDB.checkUserExist(user))
                {
                    _userDB.updateUserWithEmail(user, userEmail);
                    logActivity("Update profile", "Email = " + userEmail + ", First name = " + user.user_first_name + ", Last name = " + user.user_last_name);
                    HttpContext.Session.SetString("userEmail", user.user_email);
                }
                else
                {
                    // Return Error
                    msg = "Email has already existed. Please choose another email.";
                    status = false;
                }
            }
            HttpContext.Session.SetString("firstName", user.user_first_name);
            HttpContext.Session.SetString("lastName", user.user_last_name);
            return Ok(new
            {
                status = status,
                message = msg,
                userEmail = user.user_email,
                userFirstName = user.user_first_name,
                userLastName = user.user_last_name
            });

        }

        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var result = _userDB.getUser(HttpContext.Session.GetString("userEmail"));
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userDB.getUsers();
            return new JsonResult(users);
        }

        [HttpGet]
        public IActionResult GetAllActivities()
        {
            var activities = _activityDB.GetActivities();
            return new JsonResult(activities);
        }

        [HttpGet]
        public IActionResult GetTime()
        {
            var result = new Time();
            result.capture_time = _timeDB.getTime();
            return Ok(result);
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

        [HttpPost]
        public IActionResult UpdateTime(Models.Time.Request request)
        {
            _timeDB.updateTime(request.Time);
            return Ok();
        }      
    }
}