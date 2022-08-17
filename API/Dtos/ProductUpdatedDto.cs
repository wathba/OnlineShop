using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
 public class ProductUpdatedDto
    {
        public int Id { get; set; }
        [Required]
         public string Name { get; set; }
          [Required]
       public string Description { get; set; }
        [Required]
        [Range(100,double.PositiveInfinity)]
       public long Price { get; set; }
       public IFormFile PictureFile { get; set; }
        [Required]
       public string Type  { get; set; }
        [Required]
       public string Brand { get; set; }
        [Required]
        [Range(1,200)]
       public int QuantityInStock { get; set; }  
    }
}