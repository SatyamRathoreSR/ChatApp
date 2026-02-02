using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ChatApp.Models;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMongoCollection<Users> _usersCollection;

    // <-- Inject the collection directly
    public UsersController(IMongoCollection<Users> usersCollection)
    {
        _usersCollection = usersCollection;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        // Fetch all users (Id != null)
        var data = _usersCollection.Find(u => u.Id != null).ToList();
        return Ok(data);
    }
    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] Users user)
    {
        var exists = _usersCollection.Find(u => u.Username == user.Username).FirstOrDefault();
        if (exists != null)
            return BadRequest("Username already exists");

        _usersCollection.InsertOne(user);
        return Ok("Signup successful. Please login.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Users user)
    {

        if (!string.IsNullOrEmpty(_usersCollection.Find(u =>
            u.Username == user.Username
            && u.Password == user.Password
        ).FirstOrDefault().Username))
{
    HttpContext.Session.SetString("username", _usersCollection.Find(u =>
            u.Username == user.Username
            && u.Password == user.Password
        ).FirstOrDefault().Username!);
}
else
{
    return Unauthorized("Username is null or empty.");
}
        
        

        return Ok("Login successful");
    }
}
