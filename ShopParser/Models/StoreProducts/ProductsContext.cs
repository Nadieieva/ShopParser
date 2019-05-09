using System.Data.Entity;

namespace ShopParser.Models.StoreProducts
{
    public class ProductsContext : DbContext
    {
        public ProductsContext() : base("ShopParser")
        {
        }

        virtual public DbSet<Product> Products { get; set; }
        virtual public DbSet<Photo> Photos { get; set; }
        virtual public DbSet<HistoryEntry> History { get; set; }
    }
}