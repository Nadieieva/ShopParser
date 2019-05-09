using System.Collections.Generic;
using ShopParser.Models;

namespace ShopParser.ViewModels
{
    public class RandomItemViewModel
    {
        public Item Item { get; set; }
        public List<Customer> Customers { get; set; }
    }
}