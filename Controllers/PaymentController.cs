using DOAN.Models;
using DOAN.Services.Momo;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        
        public PaymentController( IMomoService momoService)
        {
            _momoService = momoService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfor model)
        {
            var response = await _momoService.CreatePaymentMomo(model);
            return Redirect(response.PayUrl);
        }


        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }
    }
}
