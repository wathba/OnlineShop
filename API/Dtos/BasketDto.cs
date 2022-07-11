using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class BasketDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItemDto> Items{ get; set; }
        public string PaymentIntenId { get; set; }
        public string ClientSecret { get; set; }
    }
}