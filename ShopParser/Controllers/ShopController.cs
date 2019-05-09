using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HtmlAgilityPack;
using ShopParser.Models;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using ShopParser.Models.StoreProducts;

namespace ShopParser.Controllers
{
    public class ShopController : Controller
    {
        private Helper _h;

        public ShopController(Helper h)
        {
            _h = h;
        }

        public ShopController()
        {
            _h = new Helper();
        }

        public ActionResult Index()
        {
            IEnumerable<Product> products;
            List<string> ids = _h.getProductsIds();

            var url = "https://www.6pm.com/shop";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var htmlProducts = doc.DocumentNode.SelectNodes("//body//article[@itemscope]");

            int i = 0;
            foreach (var htmlProduct in htmlProducts)
            {
                if (i > 25) break;
                i++;

                // Product URL
                string link = "https://www.6pm.com" + htmlProduct.SelectSingleNode(".//a[@itemprop='url']").Attributes["href"].Value;

                //Product ID
                string[] splittedLink = link.Split('/');
                string productId = splittedLink[splittedLink.Length - 3] + "-" + splittedLink[splittedLink.Length - 1];

                if (ids.IndexOf(productId) != -1)
                {
                    ids.RemoveAt(ids.IndexOf(productId));
                }

                //Product price
                string priceStr = htmlProduct.SelectSingleNode(".//p[@itemprop='name']/following-sibling::p").FirstChild.InnerText;
                priceStr = priceStr.Substring(1);

                priceStr = priceStr.Replace(".", ",");

                decimal price = Convert.ToDecimal(priceStr);

                Product product = _h.getProductById(productId);

                if (product == null)
                {
                    // Product name
                    string name = htmlProduct.SelectSingleNode(".//p[@itemprop='brand']/span[@itemprop='name']").InnerHtml + " "
                       + htmlProduct.SelectSingleNode(".//p[@itemprop='name']").InnerHtml;
                    name = System.Net.WebUtility.HtmlDecode(name);

                    // Product description
                    var docProduct = web.Load(link);
                    var descHtml = docProduct.DocumentNode.SelectSingleNode(".//div[@itemprop='description']//ul");
                    descHtml.ChildNodes[0].Remove();
                    string desc = descHtml.InnerHtml;

                    _h.addProduct(new Product
                    {
                        ProductId = productId,
                        ProductName = name,
                        ProductLastPrice = price,
                        ProductUrl = link,
                        ProductDecription = desc
                    });

                    //Product images                         
                    var imgsHtml = docProduct.DocumentNode.SelectNodes(".//img[@data-track-value='Image-Click']");
                    string imgUrl = "", imgName = "";
                    foreach (var imgHtml in imgsHtml)
                    {
                        imgUrl = imgHtml.Attributes["src"].Value;
                        imgName = Path.GetFileName(imgUrl);

                        byte[] data;

                        using (WebClient client = new WebClient())
                            data = client.DownloadData(imgUrl);

                        if (data != null)
                        {
                            using (MemoryStream ioStream = new MemoryStream(data))
                            {
                                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/Content/Images", imgName);
                                var fromStream = Image.FromStream(ioStream);
                                Image image = new Bitmap(fromStream);
                                fromStream.Dispose();
                                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    image.Save(ioStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] bytes = ioStream.ToArray();
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                            }
                        }

                        _h.addPhoto(new Photo
                        {
                            PhotoUrl = imgName,
                            ProductId = productId
                        });
                    }

                    // History Entry (Product's Price)
                    _h.addHistoryEntry(new HistoryEntry
                    {
                        ProductId = productId,
                        ProductPrice = price,
                        HistoryDate = DateTime.Now
                    });
                }
                else
                {
                    var history = _h.getProductHistoryById(productId);
                    HistoryEntry lastProductHistory = _h.getLastProductHistoryEntry(productId);
                    if (lastProductHistory == null || !(lastProductHistory.HistoryDate.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")))
                    {
                        _h.addHistoryEntry(new HistoryEntry
                        {
                            ProductId = productId,
                            ProductPrice = price,
                            HistoryDate = DateTime.Now
                        });
                    }
                    _h.updateProductLastPrice(product, price);
                }
            }

            foreach (string id in ids)
            {
                Product product = _h.getProductById(id);
                _h.updateProductLastPrice(product, 0);
            }
            // Get all products
            products = _h.getProductsAll();

            return View(products);
        }

        public ActionResult SingleProduct(string id)
        {
            Product product;

            // Find product with specified ID
            product = _h.getProductById(id);

            if (product != null)
            {
                ViewBag.History = product.History;
                ViewBag.Images = product.Photos;
                return View(product);
            }
            else
            {
                return View("~/Views/Shop/ProductNotFound.cshtml");
            }

        }
    }
}