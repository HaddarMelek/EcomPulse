using System;
using System.Collections.Generic;

namespace EcomPulse.Web.ViewModel
{
    public class OrderVM
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        public string UserName { get; set; }

        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; } = "Pending";

        public List<OrderItemVM> OrderItems { get; set; }

        public OrderVM()
        {
            OrderItems = new List<OrderItemVM>();
        }
    }

    
}