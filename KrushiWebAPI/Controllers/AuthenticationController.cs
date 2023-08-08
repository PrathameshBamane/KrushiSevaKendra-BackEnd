using KrushiWebAPI.context;
using KrushiWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KrushiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        DatabaseContext _context;
        IConfiguration _configuration;

        public AuthenticationController(DatabaseContext Context, IConfiguration configuration) 
        {
           _context = Context;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Admin admin )
        {
            var admins =  _context.Admins.Where(a=>a.Username == admin.Username && a.Password == admin.Password).ToList();
           if (admins.Count > 0)
            {
                var issuer = _configuration["Jwt:issuer"];
                var audience = _configuration["Jwt:audience"];
                var key =Encoding.ASCII.GetBytes
                    (_configuration["Jwt:key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id",admins[0].Id.ToString()),
                       new Claim("Name",admins[0].Name.ToString()),
                       new Claim("Username",admins[0].Username.ToString()),
                       new Claim(JwtRegisteredClaimNames.Jti,
                       Guid.NewGuid().ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);
                admins[0].TokenString = stringToken;
                return Ok(admins);


            }
            else
            {
                return Ok(admins);
            }
          
        }
        [HttpPut]
        [Route("changepassword/{id}/{oldpassword}/{newpassword}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromRoute] string oldpassword, [FromRoute] string newpassword)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null )
                return NotFound();

            if (admin.Password.Equals(oldpassword))
            {
                admin.Password = newpassword;
                await _context.SaveChangesAsync();
                return Ok(admin);
            }
            else
            {
                return NotFound();  
            }
        }
    }
}
