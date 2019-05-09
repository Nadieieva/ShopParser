using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ShopParser.Models.StoreProducts;

namespace ShopParser.Models
{
    public class Helper
    {
        private ProductsContext context;

        public Helper()
        {
            context = new ProductsContext();
            if (!context.Database.Exists())
            {
                context.Database.Create();
            }
        }

        public List<Product> getProductsAll()
        {
            return context.Products.Include(p => p.Photos).ToList();
        }

        public virtual Product getProductById(string id)
        {
            return context.Products.SingleOrDefault(p => p.ProductId == id);
        }

        public List<string> getProductsIds()
        {
            return context.Products.Select(p => p.ProductId).ToList();
        }

        public void addProduct(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
        }

        public void addPhoto(Photo photo)
        {
            context.Photos.Add(photo);
            context.SaveChanges();
        }

        public void addHistoryEntry(HistoryEntry historyEntry)
        {
            context.History.Add(historyEntry);
            context.SaveChanges();
        }

        public void updateProductLastPrice(Product product, decimal price)
        {
            product.ProductLastPrice = price;
            context.SaveChanges();
        }

        public virtual HistoryEntry getLastProductHistoryEntry(string id)
        {
            return context.History.Where(h => h.ProductId == id).OrderByDescending(h => h.HistoryEntryId).FirstOrDefault();
        }

        public List<HistoryEntry> getProductHistoryById(string id)
        {
            return context.History.Where(h => h.ProductId == id).ToList();
        }
    }

}