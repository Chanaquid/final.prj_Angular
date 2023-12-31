// using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {


        public IBasketRepository BasketRepository { get; }
        public IUnitOfWork UnitOfWork { get; }

        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            BasketRepository = basketRepo;
        }


        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get basket from the repo
            var basket = await BasketRepository.GetBasketAsync(basketId);
            //get items from product repo
            var items  = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await UnitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.ImageUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            // get delivery method from repo
            var deliveryMethod = await UnitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quntity);

            //Check to see if order exists
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
            var order = await UnitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if(order != null)
            {
                order.ShipToAddress = shippingAddress;
                order.DeliveryMethod = deliveryMethod;
                order.Subtotal = subtotal;
                UnitOfWork.Repository<Order>().Update(order);
            }
            else
            {
                //create order
                order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, basket.PaymentIntentId);
                await UnitOfWork.Repository<Order>().AddAsync(order);
            }

            
            //TODO: save to db
            var result = await UnitOfWork.Complete();

            if (result <= 0){
                return null;
            }

            //return order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await UnitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            
            return await UnitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await UnitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}