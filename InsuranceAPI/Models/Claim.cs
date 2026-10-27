using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models
{

    public enum ClaimStatus
    {
        Submitted,
        UnderReview,
        Approved,
        Denied
    }
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }
    
        public int PolicyId { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public DateTime DateSubmitted { get; set; }  = DateTime.UtcNow;

        public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;

         // Navigation property
        public Policy Policy { get; set; } = null!;
    }
}