using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Models
{
    public class InsuranceProduct
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Precision(10, 2)]
        public decimal BasePremium { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}