using Microsoft.EntityFrameworkCore;

namespace API.RequestHelper
{
 public class PagedList<T>:List<T>
    {
      
        public PagedList(List<T> items,int count,int  pageNumber,int pageSize)
        {
   PaginationData = new PaginationData
   {
    CurrentPage = pageNumber,
    PageSize = pageSize,
    TotalItems = count,
    TotalPages = (int)Math.Ceiling(count / (double)pageSize)
   };
   AddRange(items);

  }
    public PaginationData PaginationData  { get; set; }
   public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query,int pageNumber,int pageSize){
   var count = await query.CountAsync();
   var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
   return new PagedList<T>(items, count, pageNumber, pageSize);
  }
    }
}