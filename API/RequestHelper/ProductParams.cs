namespace API.RequestHelper
{
 public class ProductParams:PaginationParams
    {
        
       public string Orderby { get; set; }
        public string Search { get; set; }
        public string Brands { get; set; }
        public string Types { get; set; }
    }
}