using API.Data;
using API.Entities;
using API.Extensions;
using API.RequestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
 [ApiController]
  [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
  
  private readonly OnlineShopContext _context;

  public ProductsController(OnlineShopContext context)
      {
   _context = context;
   
  }
  [HttpGet]
   public async Task<ActionResult<PagedList<Product>>> GetAllProducts([FromQuery]ProductParams productParams)
  {
   var query = _context.Products
   .Sort(productParams.Orderby)
   .Search(productParams.Search)
   .Filter(productParams.Brands,productParams.Types)
   .AsQueryable();

   var products = await PagedList<Product>
   .ToPagedList(query, productParams.PageNumber, productParams.PageSize);
   Response.AddPaginationHeader(products.PaginationData);
   return products;


  }
  [HttpGet("{id}")]
  public async Task<ActionResult<Product>> GetProduct(int id){
     
    return await _context.Products.FindAsync(id);
  
     
   
  }


[HttpGet("filters")]

  public async Task<ActionResult> GetFilters() {
   var brands = await  _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
   var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();
   return Ok(new { brands, types });
  }
  


 }
}