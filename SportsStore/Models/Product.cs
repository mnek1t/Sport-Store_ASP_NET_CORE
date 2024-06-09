<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> sports-store-application-4

namespace SportsStore.Models
{
  public class Product
    {
        public long ProductId { get; set; }

<<<<<<< HEAD
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(8, 2)")]

        public decimal Price { get; set; }

=======
        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
>>>>>>> sports-store-application-4
        public string Category { get; set; } = string.Empty;
    }
}
