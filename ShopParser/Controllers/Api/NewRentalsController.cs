using System;
using System.Linq;
using System.Web.Http;
using ShopParser.Dtos;
using ShopParser.Models;

namespace ShopParser.Controllers.Api
{
    public class NewRentalsController : ApiController
    {
        private ApplicationDbContext _context;

        public NewRentalsController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult CreateNewRentals(NewRentalDto newRental)
        {
            var customer = _context.Customers.Single(
                c => c.Id == newRental.CustomerId);

            var items = _context.Items.Where(
                m => newRental.ItemIds.Contains(m.Id)).ToList();

            foreach (var item in items)
            {
                if (item.NumberAvailable == 0)
                    return BadRequest("Item is not available.");

                item.NumberAvailable--;

                var rental = new Rental
                {
                    Customer = customer,
                    Item = item,
                    DateRented = DateTime.Now
                };

                _context.Rentals.Add(rental);
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}