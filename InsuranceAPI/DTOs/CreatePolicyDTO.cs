
using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.DTOs
{
    public class CreatePolicyDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid insurance product.")]
        public int ProductId { get; set; }


    }
}