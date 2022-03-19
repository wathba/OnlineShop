using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
 public class OnlineShopContext : DbContext
 {
  public OnlineShopContext(DbContextOptions options) : base(options)
  {
  }
  public DbSet<Product> Products { get; set; }
   public DbSet<Basket> Baskets { get; set; }
 
 }
}