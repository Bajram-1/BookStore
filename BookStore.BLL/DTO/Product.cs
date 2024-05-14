using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(100, ErrorMessage = "The title must be less than 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "The description must be less than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The ISBN is required.")]
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "Invalid ISBN format. ISBN should start with 97, third number is 8 or 9, it can be 10 characters or 13 and finish with number or X.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "The author name is required.")]
        [StringLength(100, ErrorMessage = "The author name must be less than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "The author name must not contain numbers or special characters.")]
        public string Author { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal Price { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal Price50 { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal Price100 { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImage> ProductImages { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
