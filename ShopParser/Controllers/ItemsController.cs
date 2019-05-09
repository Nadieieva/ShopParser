using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopParser.Models;
using ShopParser.ViewModels;

namespace ShopParser.Controllers
{
    public class ItemsController : Controller
    {
        private ApplicationDbContext _context;

        public ItemsController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ViewResult Index()
        {
            if (User.IsInRole(RoleName.CanManageItems))
                return View("List");

            return View("ReadOnlyList");
        }

        [Authorize(Roles = RoleName.CanManageItems)]
        public ViewResult New()
        {
            var genres = _context.Genres.ToList();

            var viewModel = new ItemFormViewModel
            {
                Genres = genres
            };

            return View("ItemForm", viewModel);
        }

        [Authorize(Roles = RoleName.CanManageItems)]
        public ActionResult Edit(int id)
        {
            var item = _context.Items.SingleOrDefault(c => c.Id == id);

            if (item == null)
                return HttpNotFound();

            var viewModel = new ItemFormViewModel(item)
            {
                Genres = _context.Genres.ToList()
            };

            return View("ItemForm", viewModel);
        }


        public ActionResult Details(int id)
        {
            var item = _context.Items.Include(m => m.Genre).SingleOrDefault(m => m.Id == id);

            if (item == null)
                return HttpNotFound();

            return View(item);

        }


        // GET: Items/Random
        public ActionResult Random()
        {
            var item = new Item() { Name = "T-shirt" };
            var customers = new List<Customer>
            {
                new Customer { Name = "Customer 1" },
                new Customer { Name = "Customer 2" }
            };

            var viewModel = new RandomItemViewModel
            {
                Item = item,
                Customers = customers
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.CanManageItems)]
        public ActionResult Save(Item item)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ItemFormViewModel(item)
                {
                    Genres = _context.Genres.ToList()
                };

                return View("ItemForm", viewModel);
            }

            if (item.Id == 0)
            {
                item.DateAdded = DateTime.Now;
                _context.Items.Add(item);
            }
            else
            {
                var itemInDb = _context.Items.Single(m => m.Id == item.Id);
                itemInDb.Name = item.Name;
                itemInDb.GenreId = item.GenreId;
                itemInDb.NumberInStock = item.NumberInStock;
                itemInDb.ReleaseDate = item.ReleaseDate;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Items");
        }
    }
}