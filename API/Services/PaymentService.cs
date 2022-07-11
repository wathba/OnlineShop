using API.Entities;
using Stripe;

namespace API.Services
{
 public class PaymentService
    {
  private readonly IConfiguration _config;

  public PaymentService(IConfiguration config)
       {
   _config = config;
  } 
  public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var service = new PaymentIntentService();

            var intent = new PaymentIntent();

            var subtotal = basket.Items.Sum(item => (item.Quantity * item.Product.Price)/10);
            var deliveryFee = (subtotal > 100000 || subtotal == 0) ? 0 : 5000;

            if (string.IsNullOrEmpty(basket.PaymentIntenId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = subtotal + deliveryFee,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = subtotal + deliveryFee
                };
                await service.UpdateAsync(basket.PaymentIntenId, options);
            }

            return intent;
        }
    }
}