using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using ShopParser.Dtos;
using ShopParser.Models;

namespace ShopParser.Controllers.Api
{
    public class ItemsController : ApiController
    {
        private ApplicationDbContext _context;

        public ItemsController()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<ItemDto> GetItems(string query = null)
        {
            var itemsQuery = _context.Items
                .Include(m => m.Genre)
                .Where(m => m.NumberAvailable > 0);

            if (!String.IsNullOrWhiteSpace(query))
                itemsQuery = itemsQuery.Where(m => m.Name.Contains(query));

            return itemsQuery
                .ToList()
                .Select(Mapper.Map<Item, ItemDto>);
        }

        public IHttpActionResult GetItem(int id)
        {
            var item = _context.Items.SingleOrDefault(c => c.Id == id);

            if (item == null)
                return NotFound();

            return Ok(Mapper.Map<Item, ItemDto>(item));
        }

        [HttpPost]
        [Authorize(Roles = RoleName.CanManageItems)]
        public IHttpActionResult CreateItem(ItemDto itemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var item = Mapper.Map<ItemDto, Item>(itemDto);
            _context.Items.Add(item);
            _context.SaveChanges();

            itemDto.Id = item.Id;
            return Created(new Uri(Request.RequestUri + "/" + item.Id), itemDto);
        }

        [HttpPut]
        [Authorize(Roles = RoleName.CanManageItems)]
        public IHttpActionResult UpdateItem(int id, ItemDto itemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var itemInDb = _context.Items.SingleOrDefault(c => c.Id == id);

            if (itemInDb == null)
                return NotFound();

            Mapper.Map(itemDto, itemInDb);

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = RoleName.CanManageItems)]
        public IHttpActionResult DeleteItem(int id)
        {
            var itemInDb = _context.Items.SingleOrDefault(c => c.Id == id);

            if (itemInDb == null)
                return NotFound();

            _context.Items.Remove(itemInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}