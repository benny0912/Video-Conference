using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TelemeetProject.Models;

namespace TelemeetProject.Controllers.Register
{
    public class RegisterController : Controller
    {

        private readonly IUserDB _userDB;
        public RegisterController(IUserDB userDB)
        {
            _userDB = userDB;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register([Bind] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _userDB.createUser(user);
                    if(result.error)
                    {
                        TempData["msg"] = result.message;
                        return View("~/Views/Register/Index.cshtml");
                    } else
                    {
                        TempData["msg"] = result.message;

                        return RedirectToAction("Index", "Login");
                    }
                }
            } catch (Exception e)
            {
                TempData["msg"] = e.Message;
            }
            
            
            return View("~/Views/Register/Index.cshtml");
        }
    }
}