using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace InsuranceAPI.Models
{
    public enum RoleType
    {
        Admin,
        Customer
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public RoleType Role { get; set; } = RoleType.Customer;
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}