using System.ComponentModel.DataAnnotations;
using InsuranceAPI.Models;

namespace InsuranceAPI.DTOs
{
    public class UpdateClaimStatusDTO
    {
        [EnumDataType(typeof(ClaimStatus))]
        public ClaimStatus Status { get; set; }
    }
}