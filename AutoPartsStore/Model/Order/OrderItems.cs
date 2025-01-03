﻿using System.Text.Json.Serialization;
using AutoPartsStore.Model.Product;

namespace AutoPartsStore.Model.Order
{
    public class OrderItems
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public Orders? Orders { get; set; }
        [JsonIgnore]
        public Products? Products { get; set; }
    }
}
