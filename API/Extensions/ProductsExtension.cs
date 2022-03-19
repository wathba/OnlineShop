using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Extensions
{
    public static class ProductsExtension
    {
        public static IQueryable<Product> Sort(this IQueryable<Product> query, string orderby )
        {
          if (string.IsNullOrEmpty(orderby)) return query.OrderBy(p => p.Name);
         query = orderby switch
   {
    "lowprice"=>query.OrderBy(p=>p.Price),
    "highprice"=>query.OrderByDescending(p=>p.Price),
    _=>query.OrderBy(p=>p.Name)

   };
return query;

  }
  public static IQueryable<Product> Search(this IQueryable<Product> query, string searhTerm)
  {
      if(string.IsNullOrEmpty(searhTerm)) return query;
   var searchTermLower = searhTerm.Trim().ToLower();
   return query.Where(p => p.Name.ToLower().Contains(searchTermLower));
  }
  public static IQueryable<Product> Filter(this IQueryable<Product> query, string brands, string types)
  {
   var brandsList = new List<string>();
   var typesList = new List<string>();
   if(!string.IsNullOrEmpty(brands))
    brandsList.AddRange(brands.ToLower().Split(","));
    if(!string.IsNullOrEmpty(types))
    typesList.AddRange(types.ToLower().Split(","));
   query = query.Where(p => brandsList.Count == 0 || brandsList.Contains(p.Brand.ToLower()));
    query = query.Where(p => typesList.Count == 0 || typesList.Contains(p.Type.ToLower()));
   return query;
  }
    }
}