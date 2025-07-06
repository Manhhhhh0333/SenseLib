using DOAN.Models;
using DOAN.Models.Momo;

namespace DOAN.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfor model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
