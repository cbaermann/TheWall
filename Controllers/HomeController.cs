using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult CreateUser(LoginViewModel viewModel)
        {
            Console.WriteLine("############################");

            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u=>u.Email == viewModel.newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                viewModel.newUser.Password = Hasher.HashPassword(viewModel.newUser, viewModel.newUser.Password);

                dbContext.Users.Add(viewModel.newUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("InSession", viewModel.newUser.UserId);

                return RedirectToAction("Wall");


            }
                else
                {
                    Console.WriteLine("********************");
                    return View("Index");
                }
        }

         [HttpPost("login")]
        public IActionResult LoginUser(LoginViewModel viewModel)
        {
             if(ModelState.IsValid)
            {
                var dbUser = dbContext.Users.FirstOrDefault(u=>u.Email == viewModel.newLogin.loginEmail);
                if(dbUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }

                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(viewModel.newLogin, dbUser.Password, viewModel.newLogin.loginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Password does not match email");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("InSession", dbUser.UserId);

                return RedirectToAction("Wall");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("Wall")]
        public IActionResult Wall()
        {
            if(HttpContext.Session.GetInt32("InSession") == null)
            {
                TempData["error"] = "You must be logged in to access this page";
                return RedirectToAction("Index");
            }
            WallViewModel viewModel = new WallViewModel();
            viewModel.User = dbContext.Users.FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession"));
            viewModel.Messages = dbContext.Messages
                .Include(m=>m.User)
                .Include(m=>m.Comments)
                .ThenInclude(c=>c.User)
                .OrderByDescending(x=>x.CreatedAt)
                .ToList();
            return View("Wall", viewModel);
        }

        [HttpPost("writeMessage")]
        public IActionResult CreateMessage(WallViewModel viewModel)
        {
            if(HttpContext.Session.GetInt32("InSession")==null)
            {
                TempData["error"]="You must be logged in to access this page";
                return RedirectToAction("Index");
            }
            viewModel.Message.UserId = (int)HttpContext.Session.GetInt32("InSession");
            dbContext.Messages.Add(viewModel.Message);
            dbContext.SaveChanges();
            return RedirectToAction("Wall");
        }

        [HttpGet("message/delete/{MessageId}")]
        public IActionResult DeleteMessage(int MessageId)
        {
            if(HttpContext.Session.GetInt32("InSession")==null)
            {
                TempData["error"]="You must be loged in to access this";
                return RedirectToAction("Index");
            }
            Message message = dbContext.Messages
                .Include(m=>m.User)
                .FirstOrDefault(m=>m.MessageId==MessageId);

            dbContext.Messages.Remove(message);
            dbContext.SaveChanges();
            return RedirectToAction("Wall");
        }

        [HttpPost("comment/{MessageId}/create")]
        public IActionResult CreateComment(WallViewModel viewModel, int MessageId)
        {
            if(HttpContext.Session.GetInt32("InSession")==null)
            {
                TempData["error"]="You must be logged in to access this";
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                viewModel.Comment.MessageId = MessageId;
                viewModel.Comment.UserId = (int)HttpContext.Session.GetInt32("InSession");
                dbContext.Comments.Add(viewModel.Comment);
                dbContext.SaveChanges();
                return RedirectToAction("Wall");
            }
            return View("Wall");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
