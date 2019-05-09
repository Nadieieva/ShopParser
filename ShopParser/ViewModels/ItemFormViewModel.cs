using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShopParser.Models;

namespace ShopParser.ViewModels
{
    public class ItemFormViewModel
    {
        public IEnumerable<Genre> Genres { get; set; }

        public int? Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Genre")]
        [Required]
        public byte? GenreId { get; set; }

        [Display(Name = "Release Date")]
        [Required]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Number in Stock")]
        [Range(1, 20)]
        [Required]
        public byte? NumberInStock { get; set; }


        public string Title
        {
            get
            {
                return Id != 0 ? "Edit Item" : "New Item";
            }
        }

        public ItemFormViewModel()
        {
            Id = 0;
        }

        public ItemFormViewModel(Item item)
        {
            Id = item.Id;
            Name = item.Name;
            ReleaseDate = item.ReleaseDate;
            NumberInStock = item.NumberInStock;
            GenreId = item.GenreId;
        }
    }
}