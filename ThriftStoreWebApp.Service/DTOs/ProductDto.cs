using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace ThriftStoreWebApp.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string? Name { get; set; }


        [Required, MaxLength(100)]
        public string? Brand { get; set; }


        [Required, MaxLength(100)]
        public string? Gender { get; set; }


        [Required, MaxLength(100)]
        public string? Size { get; set; }


        [Required, MaxLength(100)]
        public string? Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }
        
        public bool Availability { get; set; }
    }
}
