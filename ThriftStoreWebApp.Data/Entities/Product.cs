using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ThriftStoreWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }


        [MaxLength(100)]
        public string? Name { get; set; }


        [MaxLength(100)]
        public string? Brand { get; set; }


        [MaxLength(100)]
        public string? Gender { get; set; }


        [MaxLength(100)]
        public string? Size { get; set; }


        [MaxLength(100)]
        public string? Category { get; set; }


        [Precision(16, 2)]
        public decimal Price { get; set; }


        public string? Description { get; set; }


        [MaxLength(100)]
        public string? ImageFileName { get; set; }


        public DateTime CreatedDate { get; set; }

        public bool Availability { get; set; }
    }
}
