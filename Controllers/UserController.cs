using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoServer.DTOs;
using TodoServer.Models;
using TodoServer.Repositories;

namespace TodoServer.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private IConfiguration _configuration;
    private IUserRepository _user;
    private readonly ILogger<UserController> _logger;

    public UserController(IConfiguration configuration, IUserRepository user, ILogger<UserController> logger)
    {
        _configuration = configuration;
        _user = user;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromBody] UserDTO data)
    {
        if(!IsValidEmailAddress(data.Email))
            return BadRequest("Invalid email address");
        var toCreateUser = new User
        {
            Name = data.Name,
            Email = data.Email,
            Password = data.Password,
        };
        var createdUser = await _user.Create(toCreateUser);
        return StatusCode(StatusCodes.Status201Created, createdUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDTO userLogin)
    {
        if(!IsValidEmailAddress(userLogin.Email))
            return BadRequest("Invalid email address");

        var user = await _user.GetByEmail(userLogin.Email);
        if (user == null)
            return NotFound("User not found");
        if (!user.Password.Equals(userLogin.Password))
             return Unauthorized("Invalid password");
        var token = Generate(user);
        return Ok(token);
    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private bool IsValidEmailAddress(string email)
    {
    try
    {
        var emailChecked = new System.Net.Mail.MailAddress(email);
        return true;
    }
    catch
    {
        return false;
    }
    }

}
