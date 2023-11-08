using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public interface IPaymentService
  {
    Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
    Task<Entities.OrderAggregate.Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
    Task<Entities.OrderAggregate.Order> UpdateOrderPaymentFailed(string paymentIntentId);
  }
}