using API.Data;
using API.Entities;
using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Extensions;

namespace API.Controllers
{
 [ApiController]
    [Route("api/[Controller]")]
    public class BasketController:ControllerBase
    {
  private readonly OnlineShopContext _context;
       public BasketController(OnlineShopContext context)
       {
   _context = context;
           
       }
       //get basket
 [HttpGet( Name ="GetBasket")] 
  public async Task<ActionResult<BasketDto>> GetBasket()
  {
   var basket = await RetrieveBasket(GetBuyerId());
   if (basket == null) return NotFound();
   return basket.GetMapBasketDto();
  }

//add basket item

  [HttpPost]
  public async  Task<ActionResult> AddItemToBasket(int productId, int quantity)
  {
   var basket = await  RetrieveBasket(GetBuyerId());
   if(basket==null)  basket = CreateBasket();
   
   var product = await _context.Products.FindAsync(productId);
   if(product ==null) return NotFound();
   basket.AddItem(product, quantity);
   var result = await _context.SaveChangesAsync() > 0;
  if(result) return  CreatedAtRoute("GetBasket",basket.GetMapBasketDto());
   return BadRequest(new ProblemDetails
   {
    Title = "Problem saving item"
   });
  }

 // remove basket

  [HttpDelete]
  public async  Task<ActionResult> RemoveBasketItem(int  productId, int quantity)
  {
   var basket = await  RetrieveBasket(GetBuyerId());
   if(basket==null)return NotFound();
   basket.RemoveItem(productId, quantity);
   var result = await _context.SaveChangesAsync() > 0;
   if(result) return Ok();
   return BadRequest( new ProblemDetails{
       Title=" No Product To Remove"
   });

  }
// retrieve Basket
    private async  Task<Basket> RetrieveBasket(string buyerId)
  {
    if(string.IsNullOrEmpty(buyerId)){
    Response.Cookies.Delete("buyerId");
    return null;
   }
   return await  _context.Baskets
      .Include(b => b.Items)
      .ThenInclude(p => p.Product)
      .FirstOrDefaultAsync(b => b.BuyerId == buyerId);
  }
  //get buyerId
  private string GetBuyerId(){
   return User.Identity?.Name ?? Request.Cookies["buyerId"];
  }
  // create Basket Method
   private Basket CreateBasket()
  {
   var buyerId = User.Identity?.Name;
   if(string.IsNullOrEmpty(buyerId)){
    buyerId = Guid.NewGuid().ToString();
     var cookiesOptions = new CookieOptions
   {
    IsEssential = true,
    Expires = DateTime.Now.AddDays(30)
   };
   Response.Cookies.Append("buyerId", buyerId, cookiesOptions);
   }
  
   var basket = new Basket
   {
    BuyerId = buyerId
   };
   _context.Baskets.Add(basket);
   return basket;
  }


  
 
    }

}