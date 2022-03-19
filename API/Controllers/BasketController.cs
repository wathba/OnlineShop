using API.Data;
using API.Entities;
using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
       [HttpGet( Name ="GetBasket")] 
       public async Task<ActionResult<BasketDto>> GetBasket()
  {
   var basket = await RetrieveBasket();
   if (basket == null) return NotFound();
   return GetBasketDto(basket);
  }



  [HttpPost]
  public async  Task<ActionResult> AddItemToBasket(int productId, int quantity)
  {
   var basket = await  RetrieveBasket();
   if(basket==null) basket = CreateBasket();
   
   var product = await _context.Products.FindAsync(productId);
   if(product ==null) return NotFound();
   basket.AddItem(product, quantity);
   var result = await _context.SaveChangesAsync() > 0;
  if(result) return  CreatedAtRoute("GetBasket",GetBasketDto(basket));
   return BadRequest(new ProblemDetails
   {
    Title = "Problem saving item"
   });
  }

 

  [HttpDelete]
  public async  Task<ActionResult> RemoveBasketItem(int  productId, int quantity)
  {
   var basket = await RetrieveBasket();
   if(basket==null)return NotFound();
   basket.RemoveItem(productId, quantity);
   var result = await _context.SaveChangesAsync() > 0;
   if(result) return Ok();
   return BadRequest( new ProblemDetails{
       Title=" No Product To Remove"
   });

  }
// retrieve Basket
    private  Task<Basket> RetrieveBasket()
  {
   return  _context.Baskets
      .Include(b => b.Items)
      .ThenInclude(p => p.Product)
      .FirstOrDefaultAsync(b => b.BuyerId == Request.Cookies["buyerId"]);
  }
  // create Basket Method
   private Basket CreateBasket()
  {
   var buyerId = Guid.NewGuid().ToString();
   var cookiesOptions = new CookieOptions
   {
    IsEssential = true,
    Expires = DateTime.Now.AddDays(30)
   };
   Response.Cookies.Append("buyerId", buyerId, cookiesOptions);
   var basket = new Basket
   {
    BuyerId = buyerId
   };
   _context.Baskets.Add(basket);
   return basket;
  }


  //basketdto function
    private static BasketDto GetBasketDto(Basket basket)
  {
   return new BasketDto
   {
    Id = basket.Id,
    BuyerId = basket.BuyerId,
    Items = basket.Items.Select(Item => new BasketItemDto
    {
     ProductId = Item.ProductId,
     Name = Item.Product.Name,
     Price = Item.Product.Price,
     PictureUrl = Item.Product.PictureUrl,
     Brand = Item.Product.Brand,
     Type = Item.Product.Type,
     Quantity = Item.Quantity
    }).ToList()

   };
  }
    }

}