using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            
        }
        public OrderItem(ProductItemOrdered itemOrdered, decimal price, int quntity)
        {
            ItemOrdered = itemOrdered;
            Price = price;
            Quntity = quntity;
        }
        public ProductItemOrdered ItemOrdered {get; set;}
        public decimal Price {get; set;}
        public int Quntity {get; set;}
    }
}