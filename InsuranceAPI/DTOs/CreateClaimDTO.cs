
using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.DTOs
{
    public class CreateClaimDTO
    {
        [Range(1, int.MaxValue)]
        public int PolicyId { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
    }
}