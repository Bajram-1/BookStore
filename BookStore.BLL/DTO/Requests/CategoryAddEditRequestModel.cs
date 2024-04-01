using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO.Requests
{
    public class CategoryAddEditRequestModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Category Name must be less than or equal to 30 characters")]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}