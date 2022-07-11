using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class BasketExtension
    {
        public static BasketDto GetMapBasketDto(this Basket basket){
        return new BasketDto
   {
    Id = basket.Id,
    BuyerId = basket.BuyerId,
    PaymentIntenId=basket.PaymentIntenId,
    ClientSecret=basket.ClientSecret,
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
        public  static IQueryable<Basket> RetreiveBasketWithItems (this IQueryable<Basket> query, string buyerId){

   return query.Include(b => b.Items).ThenInclude(bi => bi.Product)
            .Where(b => b.BuyerId == buyerId);
  }
    }
}