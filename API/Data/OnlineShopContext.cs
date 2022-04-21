using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
 public class OnlineShopContext : IdentityDbContext<User>
 {
  public OnlineShopContext(DbContextOptions options) : base(options)
  {
  }
  public DbSet<Product> Products { get; set; }
   public DbSet<Basket> Baskets { get; set; }
   protected override void OnModelCreating(ModelBuilder builder)
   {
   base.OnModelCreating(builder);
   builder.Entity<IdentityRole>()
   .HasData(
    new IdentityRole{Name="Member",NormalizedName="MEMBER"},
    new IdentityRole{Name="Admin",NormalizedName="ADMIN"}
   );
  }
 
 }
}