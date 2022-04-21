using API.Data;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
 [ApiController]
 [Route("api/[Controller]")]
 public class AccountController : ControllerBase
 {
  private readonly UserManager<User> _userManager;
  private readonly TokenService _tokenService;
  private readonly OnlineShopContext _context;

  public AccountController(UserManager<User> userManager, TokenService tokenService,OnlineShopContext context )
  {
   _context = context;
   _tokenService = tokenService;
   _userManager = userManager;

  }
  [HttpPost("register")]
  public async Task<ActionResult> Register(RegisterDto registerDto)
  {
   var user = new User
   {
    UserName = registerDto.Username,
    Email = registerDto.Email
   };
   var result = await _userManager.CreateAsync(user, registerDto.Password);
   if (!result.Succeeded)
   {
    foreach (var error in result.Errors)
    {
     ModelState.AddModelError(error.Code, error.Description);
    }
    return ValidationProblem();
   }
   await _userManager.AddToRoleAsync(user, "Member");
   return StatusCode(201);
  }
  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
   var user = await _userManager.FindByNameAsync(loginDto.Username);
   if (user == null ||!(await _userManager.CheckPasswordAsync(user, loginDto.Password)))
    return Unauthorized();
   var userBasket = await  RetrieveBasket(user.UserName);
   var anonBasket =  await RetrieveBasket(Request.Cookies["buyerId"]);
   if(anonBasket!=null){
      if(userBasket!=null) _context.Baskets.Remove(userBasket);
    anonBasket.BuyerId = user.UserName;
    Response.Cookies.Delete("buyerId");
    await _context.SaveChangesAsync();

   }

   return new UserDto
   {
    Email = user.Email,
    Token = await _tokenService.TokenGeneration(user),
   Basket = anonBasket != null ? anonBasket.GetMapBasketDto() : userBasket?.GetMapBasketDto()
  };


  }


    private async Task<Basket> RetrieveBasket(string buyerId)
  {
    if(string.IsNullOrEmpty(buyerId)){
    Response.Cookies.Delete("buyerId");
    return null;
   }
   return  await _context.Baskets
      .Include(b => b.Items)
      .ThenInclude(p => p.Product)
      .FirstOrDefaultAsync(b => b.BuyerId == buyerId);
  }
  
  [Authorize]
  [HttpGet("current-user")]
  public async Task<ActionResult<UserDto>> GetCurrentUser(){
   var user = await _userManager.FindByNameAsync(User.Identity.Name);
   var userBasket = await RetrieveBasket(user.UserName);
   return new UserDto
   {
    Email = user.Email,
    Token = await _tokenService.TokenGeneration(user),
    Basket = userBasket?.GetMapBasketDto()
  };
  }
 }
}