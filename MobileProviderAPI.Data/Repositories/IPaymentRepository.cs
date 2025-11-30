using MobileProviderAPI.Models;

namespace MobileProviderAPI.Data.Repositories;

public interface IPaymentRepository
{
    Task<Payment> AddPaymentAsync(Payment payment);
}

