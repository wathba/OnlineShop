using System.Text.Json;
using API.RequestHelper;

namespace API.Extensions
{
 public static class HttpHeaderExtension
    {
        public static void AddPaginationHeader(this HttpResponse httpResponse, PaginationData paginationData){
   var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
   httpResponse.Headers.Add("pagination", JsonSerializer.Serialize(paginationData, options));
   httpResponse.Headers.Add("Access-Control-Expose-Headers", "pagination");
  }
    }
}