using System.Collections.Generic;
using System;

namespace _23DH111923_MyStore.Models
{
    public partial class Order
    {
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingAddress { get; set; }
    }
}