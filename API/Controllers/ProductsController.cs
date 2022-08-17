using API.Data;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.RequestHelper;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
 [ApiController]
  [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
  
  private readonly OnlineShopContext _context;
  
  
  private readonly IMapper _mapper;
  private readonly ImageService _imageService;

  public ProductsController(OnlineShopContext context,IMapper mapper,ImageService imageService)
      {
   _imageService = imageService;
   _mapper = mapper;

   
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
  [HttpGet("{id}",Name ="GetProduct")]
  public async Task<ActionResult<Product>> GetProduct(int id){
     
    return await _context.Products.FindAsync(id);
  
     
   
  }


[HttpGet("filters")]

  public async Task<ActionResult> GetFilters() {
   var brands = await  _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
   var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();
   return Ok(new { brands, types });
  }
  [Authorize(Roles ="Admin")]
  [HttpPost()]
  public async Task<ActionResult<Product>> AddProduct([FromForm]ProductCreatedDto productCreatedDto){
   var product = _mapper.Map<Product>(productCreatedDto);
   if(productCreatedDto.PictureFile!=null){
var imageResult = await _imageService.AddImageAsync(productCreatedDto.PictureFile);
if(imageResult.Error!=null) return BadRequest(new ProblemDetails { Title = imageResult.Error.Message});
    product.PictureUrl = imageResult.SecureUrl.ToString();
    product.PublicId = imageResult.PublicId;
   }
   _context.Products.Add(product);
   var result = await _context.SaveChangesAsync() > 0;
   if(result)return CreatedAtRoute("GetProduct", new { Id = product.Id }, product);
   return BadRequest(new ProblemDetails { Title="Failed To Add this Product try again" });
  }
  [Authorize(Roles ="Admin")]
[HttpPut()]
public async Task<ActionResult> UpdateProduct([FromForm]ProductUpdatedDto productUpdatedDto){
   var product = await _context.Products.FindAsync(productUpdatedDto.Id);
   if(product==null)return NotFound();
   _mapper.Map(productUpdatedDto,product);
    if(productUpdatedDto.PictureFile!=null){
var imageResult = await _imageService.AddImageAsync(productUpdatedDto.PictureFile);
if(imageResult.Error!=null) return BadRequest(new ProblemDetails { Title = imageResult.Error.Message});
if(!string.IsNullOrEmpty(product.PublicId))
await _imageService.DeleteImageAsync(product.PublicId);
    product.PictureUrl = imageResult.SecureUrl.ToString();
    product.PublicId = imageResult.PublicId;
   }
   var result =  await _context.SaveChangesAsync() > 0;
   if(result) return Ok(product);
   return BadRequest(new ProblemDetails { Title = "problem updating this product" });
  }
[Authorize(Roles ="Admin")]
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteProduct(int id){
   var product = await _context.Products.FindAsync(id);
   if(product==null)return NotFound();
   if(!string.IsNullOrEmpty(product.PublicId))
await _imageService.DeleteImageAsync(product.PublicId);
    _context.Products.Remove(product);
   var result =  await _context.SaveChangesAsync() > 0;
   if(result) return Ok();
   return BadRequest(new ProblemDetails { Title = "problem Deleting this product" });
  }
 }
}