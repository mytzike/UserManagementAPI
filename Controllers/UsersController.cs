using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System;
using UserManagementAPI.Models;

[Authorize] // All endpoints require authentication unless [AllowAnonymous] is used
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // In-memory user store
    private static Dictionary<int, User> usersDict = new Dictionary<int, User>
    {
        { 1, new User { Id = 1, Name = "Alice", Email = "alice@example.com" } },
        { 2, new User { Id = 2, Name = "Bob", Email = "bob@example.com" } }
    };

    // Public: Get all users
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers() => Ok(usersDict.Values);

    // Protected: Get user by ID
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        try
        {
            return usersDict.TryGetValue(id, out var user) ? Ok(user) : NotFound("User not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Protected: Create a new user
    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
            return BadRequest("Name and Email are required.");

        int newId = usersDict.Keys.Any() ? usersDict.Keys.Max() + 1 : 1;
        user.Id = newId;
        usersDict[newId] = user;
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Protected: Update user
    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, User updatedUser)
    {
        if (!usersDict.ContainsKey(id)) return NotFound("User not found");

        usersDict[id].Name = updatedUser.Name;
        usersDict[id].Email = updatedUser.Email;
        return NoContent();
    }

    // Protected: Delete user
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        if (!usersDict.ContainsKey(id)) return NotFound("User not found");

        usersDict.Remove(id);
        return NoContent();
    }

    // Public: Generate JWT token for testing
    [AllowAnonymous]
    [HttpGet("generate-token")]
    public ActionResult<string> GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "UserManagementAPI",
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}

