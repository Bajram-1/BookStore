using BookStore.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must only contain letters and spaces.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Street Address is required.")]
        [StringLength(200, ErrorMessage = "Street address must not exceed 200 characters.")]
        public string StreetAddress { get; set; }
        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must only contain letters and spaces.")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State name must not exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State must only contain letters and spaces.")]
        public string State { get; set; }
        [Required(ErrorMessage = "Postal Code is required.")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Postal code must be a valid ZIP code.")]
        public string PostalCode { get; set; }
        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        [StringLength(50, ErrorMessage = "Role name must not exceed 50 characters.")]
        public string? Role { get; set; }
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; } = new List<OrderHeader>();
    }
}
