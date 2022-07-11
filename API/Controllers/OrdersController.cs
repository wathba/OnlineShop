using API.Data;
using API.Dtos;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
       
  private readonly OnlineShopContext _context;

        public OrdersController(OnlineShopContext context)
        {
   _context = context;
           
        }

     [HttpGet]
     public async Task<ActionResult<List<OrderDto>>> GetOrders(){
   return await _context.Orders
   .MapOrderToDto()
   .Where(o => o.BuyerId == User.Identity.Name)
   .ToListAsync();
  }
     [HttpGet("id",Name ="GetOrder")]
     public async Task<ActionResult<List<OrderDto>>> GetOrder(int id){
   return await _context.Orders
   .MapOrderToDto()
   .Where(o => o.BuyerId == User.Identity.Name&& o.Id==id)
   .ToListAsync();
  }
    [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto)
        {
            var basket = await _context.Baskets
                .RetreiveBasketWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            if (basket == null) return BadRequest(new ProblemDetails { Title = "Could not locate basket" });

            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var productItem = await _context.Products.FindAsync(item.ProductId);
                var itemOrdered = new ProductItemOrder
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };
             if(productItem.QuantityInStock < item.Quantity)
     return BadRequest(new ProblemDetails
     {
         Title="sorry we do not have the quantity "

     });
    var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);
                productItem.QuantityInStock -= item.Quantity;
            }

            var subtotal = items.Sum(item => item.Price * item.Quantity);
            var deliveryFee = subtotal > 100000 ? 0 : 5000;

            var order = new Order
            {
                OrderItems = items,
                BuyerId = User.Identity.Name,
                ShippingAddress = orderDto.ShippingAddress,
                Subtotal = subtotal,
                DeliveryFee = deliveryFee,
            };

            _context.Orders.Add(order);
            _context.Baskets.Remove(basket);

            if (orderDto.SaveAddress)
            {
                var user = await _context.Users
                    .Include(a => a.Address)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                var address = new UserAddress
                {
                    FullName = orderDto.ShippingAddress.FullName,
                    Address1 = orderDto.ShippingAddress.Address1,
                    Address2 = orderDto.ShippingAddress.Address2,
                    City = orderDto.ShippingAddress.City,
                    Country = orderDto.ShippingAddress.Country,
                    ZipCode = orderDto.ShippingAddress.ZipCode,
                    Phone = orderDto.ShippingAddress.Phone
                };
                user.Address = address;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);

            return BadRequest(new ProblemDetails { Title = "Problem creating order" });
        }

    }
}