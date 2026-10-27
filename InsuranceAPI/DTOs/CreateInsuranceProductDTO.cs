using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceAPI.DTOs
{
    public class CreateInsuranceProductDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "99999999.99")]
        public decimal BasePremium { get; set; }

        
    }
}