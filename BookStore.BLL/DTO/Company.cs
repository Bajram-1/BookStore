using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.DTO
{
    public class Company
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must only contain letters and spaces.")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "Street address must not exceed 200 characters.")]
        public string? StreetAddress { get; set; }
        [StringLength(100, ErrorMessage = "City name must not exceed 100 characters.")]
        public string? City { get; set; }
        [StringLength(50, ErrorMessage = "State name must not exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State must only contain letters and spaces.")]
        public string? State { get; set; }
        [RegularExpression(@"^[A-Za-z0-9\s\-]{3,10}$", ErrorMessage = "Postal code must be between 3 and 10 characters, including letters, digits, spaces, or hyphens.")]
        public string? PostalCode { get; set; }
        [RegularExpression(@"^\+?\d{1,3}[\s-]?\d{1,4}[\s-]?\d{1,4}[\s-]?\d{1,4}[\s-]?\d{1,9}$", ErrorMessage = "Phone number must be in a valid international format.")]
        public string? PhoneNumber { get; set; }
    }
}
