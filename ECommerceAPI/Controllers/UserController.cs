using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ECommerceAPI.Controllers

{
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
private readonly ECommerceDbContext _context;
private readonly AuthSettings _authSettings;
public UserController(ECommerceDbContext context, IOptions<AuthSettings>
authSettings)
{
_context = context;
_authSettings = authSettings.Value;
}
// POST: api/User/register
[HttpPost("register")]
public async Task<IActionResult> Register(User user)
{
if (await _context.Users.AnyAsync(u => u.Username == user.Username))
{
return BadRequest("Username already exists.");
}
user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
_context.Users.Add(user);
await _context.SaveChangesAsync();
return Ok("User registered successfully.");
}
// POST: api/User/login
[HttpPost("login")]
public async Task<IActionResult> Login(User user)
{
var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username ==
user.Username);
if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash,
dbUser.PasswordHash))
{
return Unauthorized("Invalid username or password.");
}
var token = GenerateJwtToken(dbUser);
return Ok(new { Token = token });
}
private string GenerateJwtToken(User user)
{
var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.UTF8.GetBytes(_authSettings.Secret);
var tokenDescriptor = new SecurityTokenDescriptor
{
Subject = new ClaimsIdentity(new Claim[]
{
new Claim(ClaimTypes.Name, user.Id.ToString())
}),
Expires = DateTime.UtcNow.AddHours(1),
SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
SecurityAlgorithms.HmacSha256Signature)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
return tokenHandler.WriteToken(token);
}
}
}