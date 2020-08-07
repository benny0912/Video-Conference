using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelemeetProject.Models;

namespace TelemeetProject.Controllers.Login
{
    public class LoginController : Controller
    {

        private readonly IUserDB _userDB;
        private readonly IActivityDB _activityDB;

        public LoginController(IUserDB userDB, IActivityDB activityDB)
        {
            _userDB = userDB;
            _activityDB = activityDB;
        }
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userEmail")))
            {
                return RedirectToAction("Index", "Main");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Login([Bind] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _userDB.loginUser(user);
                    if (result.error)
                    {
                        TempData["msg"] = result.message;
                        return View("~/Views/Login/Index.cshtml");
                    }
                    else
                    {
                        var temp = _userDB.getUser(user);
                        if(temp != null)
                        {
                            _userDB.updateUserSignIn(temp.user_email);
                            HttpContext.Session.SetString("firstName", temp.user_first_name);
                            HttpContext.Session.SetString("lastName", temp.user_last_name);
                            HttpContext.Session.SetString("userEmail", temp.user_email);
                            HttpContext.Session.SetString("userRole", temp.user_role);

                            Activity activity = new Activity
                            {
                                user_email = temp.user_email,
                                user_first_name = temp.user_first_name,
                                user_last_name = temp.user_last_name,
                                activity_type = "Sign in",
                                activity_details = "",
                                last_activity = DateTime.Now,
                            };
                            _activityDB.logActivity(activity);
                            TempData["msg"] = result.message;
                            return RedirectToAction("Index", "Main");
                        } else
                        {
                            TempData["msg"] = "Failed to retrieve user information.";
                            return View("~/Views/Login/Index.cshtml");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["msg"] = e.Message;
            }
            return View("~/Views/Login/Index.cshtml");
        }

        public IActionResult Logout()
        {
            Activity activity = new Activity
            {
                user_email = HttpContext.Session.GetString("userEmail"),
                user_first_name = HttpContext.Session.GetString("firstName"),
                user_last_name = HttpContext.Session.GetString("lastName"),
                activity_type = "Log out",
                activity_details = "",
                last_activity = DateTime.Now,
            };
            _activityDB.logActivity(activity);
            HttpContext.Session.Remove("firstName");
            HttpContext.Session.Remove("lastName");
            HttpContext.Session.Remove("userRole");
            HttpContext.Session.Remove("userEmail");
            TempData["msg"] = "Successfully logged out.";
            return RedirectToAction("Index", "Login");
        }
    
    }
}
