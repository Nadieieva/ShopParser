using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopParser.Controllers;
using System.Web.Mvc;
using Moq;
using System.Data.Entity;
using System.Linq;
using ShopParser.Models;
using System.Web;
using System.IO;
using ShopParser.Models.StoreProducts;

namespace ShopParser.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void IndexViewResultNotNull()
        {
            ShopController controller = new ShopController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SingleProductViewResultNotNull()
        {
            ShopController controller = new ShopController();
            ViewResult result = controller.SingleProduct("45") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProductNotFoundTest()
        {
            var mockHelper = new Mock<Helper>();
            mockHelper.Setup(h => h.getProductById(It.IsAny<string>())).Returns<Product>(null);

            ShopController controller = new ShopController(mockHelper.Object);

            ViewResult result = controller.SingleProduct("i'm not exist") as ViewResult;
            Assert.AreEqual("~/Views/Shop/ProductNotFound.cshtml", result.ViewName);
        }

    }
}
