using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Models
{
    public enum PolicyStatus
    {
        Pending,
        Active,
        Expired,
        Cancelled
    }
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public PolicyStatus Status { get; set; } = PolicyStatus.Pending;

        // Navigation properties
        public User User { get; set; } = null!;
        public InsuranceProduct Product { get; set; } = null!;
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}