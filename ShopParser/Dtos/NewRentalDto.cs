using System.Collections.Generic;

namespace ShopParser.Dtos
{
    public class NewRentalDto
    {
        public int CustomerId { get; set; }
        public List<int> ItemIds { get; set; }
    }
}