using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using InsuranceAPI.Data;
using InsuranceAPI.Models;
using InsuranceAPI.DTOs;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure that all endpoints in this controller require authentication
    public class PolicyController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public PolicyController(InsuranceDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> CreatePolicy(CreatePolicyDTO dto)
        {

            var product = await _context.InsuranceProducts.FindAsync(dto.ProductId);
            if (product == null || !product.IsActive)
            {
                return BadRequest("Insurance Product not found or is inactive.");
            }

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var startDate = DateTime.UtcNow;

            var policy = new Policy
            {
                UserId = userId,
                ProductId = dto.ProductId,
                StartDate = startDate,
                EndDate = startDate.AddYears(1),
                Status = PolicyStatus.Pending
            };

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Policy created successfully.",
                policyId = policy.PolicyId
            });
        }

        [HttpGet("MyPolicies")]
        public async Task<IActionResult> GetMyPolicies()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var policies = await _context.Policies
                .Where(p => p.UserId == userId)
                .Include(p => p.Product)
                .ToListAsync();

            var result = policies.Select(p => new
            {
                p.PolicyId,
                p.ProductId,
                ProductName = p.Product.Name,
                p.StartDate,
                p.EndDate,
                p.Status
            });

            return Ok(result);

        }

        [HttpGet("AllPolicies")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var policies = await _context.Policies
                .Include(p => p.Product)
                .OrderByDescending(p => p.PolicyId)
                .Select(p => new
                {
                    p.PolicyId,
                    p.UserId,
                    p.ProductId,
                    ProductName = p.Product.Name,
                    p.StartDate,
                    p.EndDate,
                    p.Status
                })
                .ToListAsync();

            return Ok(policies);
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivatePolicy(int id)
        {
            var policy = await _context.Policies.FindAsync(id);

            if (policy == null || policy.Status != PolicyStatus.Pending)
            {
                return BadRequest("Only pending policies can be activated.");
            }

            policy.Status = PolicyStatus.Active;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Policy activated successfully." });
        }


        

    }
}