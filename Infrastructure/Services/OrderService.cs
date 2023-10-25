using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {

        public IGenericRepository<Order> OrderRepo { get; }
        public IGenericRepository<DeliveryMethod> DmRepo { get; }
        public IGenericRepository<Core.Entities.Product> ProductRepo { get; }
        public IBasketRepository BasketRepository { get; }

        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<DeliveryMethod> dmRepo, 
            IGenericRepository<Core.Entities.Product> productRepo, IBasketRepository basketRepo)
        {
            OrderRepo = orderRepo;
            DmRepo = dmRepo;
            ProductRepo = productRepo;
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
                var productItem = await ProductRepo.GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.ImageUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            // get delivery method from repo
            var deliveryMethod = await DmRepo.GetByIdAsync(deliveryMethodId);
            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quntity);
            //create order
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
            
            //TODO: save to db

            //return order
            return order;
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByIdAync(int id, string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}