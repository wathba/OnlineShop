using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
 public class TokenService
 {
  private readonly IConfiguration _configuration;
  private readonly UserManager<User> _userManager;
  public TokenService(UserManager<User> userManager, IConfiguration configuration)
  {
   _userManager = userManager;
   _configuration = configuration;
  }

  public async Task<string> TokenGeneration(User user)
  {
   var claims = new List<Claim>
      {
          new Claim(ClaimTypes.Email,user.Email),
          new Claim(ClaimTypes.Name,user.UserName)
      };
   var roles =  await _userManager.GetRolesAsync(user);
   foreach (var role in roles)
   {
    claims.Add(new Claim(ClaimTypes.Role, role));
   }
   var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:TokenKey"]));
   var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
   var token = new JwtSecurityToken(
       issuer: null,
       audience: null,
       claims:claims,
       expires:DateTime.Now.AddDays(7),
       signingCredentials:creds



   );
   return new JwtSecurityTokenHandler().WriteToken(token);
  }
 }
}