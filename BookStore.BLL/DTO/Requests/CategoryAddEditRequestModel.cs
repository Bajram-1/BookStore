﻿using System;
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

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, ErrorMessage = "Category name must be less than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name should only contain letters and spaces.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        private int _displayOrder;

        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be a positive number.")]
        public int DisplayOrder
        {
            get { return _displayOrder; }
            set { _displayOrder = value > 0 ? value : 1; }
        }
    }
}