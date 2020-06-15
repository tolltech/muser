using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tolltech.MuserUI.Kontur.PaymentOrder;

namespace Tolltech.MuserUI.Controllers
{
    [AllowAnonymous]
    [Route("kontur")]
    public class KonturController : BaseController
    {
        private readonly IPaymentOrderConverter paymentOrderConverter;

        public KonturController(IPaymentOrderConverter paymentOrderConverter)
        {
            this.paymentOrderConverter = paymentOrderConverter;
        }

        [HttpGet("paymentorder")]
        public IActionResult PaymentOrderForm()
        {
            return View();
        }

        [HttpPost("paymentorder")]
        public string GetPaymentOrder(string paymentOrderText)
        {
            return paymentOrderConverter.Convert(paymentOrderText);
        }
    }
}