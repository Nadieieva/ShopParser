using System;

namespace ShopParser.Models.StoreProducts
{
    public class HistoryEntry
    {
        public int HistoryEntryId { get; set; }


        public string ProductId { get; set; }
        public virtual Product Product { get; set; }

        public decimal ProductPrice { get; set; }
        public DateTime HistoryDate { get; set; }
    }
}