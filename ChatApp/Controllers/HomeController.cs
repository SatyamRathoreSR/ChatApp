using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ChatApp.Models;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<Users> _usersCollection;

        private readonly IMongoCollection<Chats> _chatsCollection;

        public HomeController(
        IMongoCollection<Users> usersCollection,
        IMongoCollection<Chats> chatsCollection)
        {
            _usersCollection = usersCollection;
            _chatsCollection = chatsCollection;
        }

        public IActionResult Index()
        {
            var users = _usersCollection.Find(u => u.Id != null).ToList();
            return View(users); // pass users list to the view
        }

        // SIGNUP
        [HttpPost]
        public IActionResult Signup(string username, string password)
        {
            var exists = _usersCollection.Find(u => u.Username == username).FirstOrDefault();

            if (exists != null)
            {
                ViewBag.Message = "Username already exists";
                return View("Index");
            }

            _usersCollection.InsertOne(new Users
            {
                Username = username,
                Password = password
            });

            ViewBag.Message = "Signup successful. Please login.";
            return View("Index");
        }

        // LOGIN
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _usersCollection.Find(u =>
                u.Username == username &&
                u.Password == password
            ).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Message = "Could not find the data or the password was wrong";
                return View("Index");
            }

            HttpContext.Session.SetString("username", user.Username);
            return RedirectToAction("Chat");
        }

        public IActionResult Chat()
        {
            var me = HttpContext.Session.GetString("username");
        if (me == null)
            return RedirectToAction("Index");

        ViewBag.LoggedInUser = me;

        var users = _usersCollection.Find(u => u.Username != me).ToList();
        return View(users); // Chats.cshtml
        }
        [HttpPost]
    public IActionResult SendMessage(string to, string message)
    {
        var from = HttpContext.Session.GetString("username");
        if (from == null) return Unauthorized();

        var chat = new Chats
        {
            From = from,
            To = to,
            Message = message,
            Timestamp = DateTime.Now
        };

        _chatsCollection.InsertOne(chat);
        return Ok();
    }
    [HttpGet]
public IActionResult GetChats(string withUser)
        {
        var me = HttpContext.Session.GetString("username");
        if (me == null) return Unauthorized();

        var chats = _chatsCollection.Find(c =>
         (c.From == me && c.To == withUser) ||
         (c.From == withUser && c.To == me)
        )
        .SortBy(c => c.Timestamp)
        .ToList();

       return PartialView("_ChatMessages", chats);
        }



    }
}