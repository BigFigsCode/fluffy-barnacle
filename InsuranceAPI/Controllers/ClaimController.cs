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
    [Authorize]
    public class ClaimController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        public ClaimController(InsuranceDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateClaim(CreateClaimDTO dto)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var policy = await _context.Policies.FirstOrDefaultAsync(p => p.PolicyId == dto.PolicyId && p.UserId == userId);
            if (policy == null || policy.Status != PolicyStatus.Active)
            {
                return BadRequest("Policy not found or is not active.");
            }


            var claim = new InsuranceAPI.Models.Claim
            {
                PolicyId = dto.PolicyId,
                Description = dto.Description,
                Status = ClaimStatus.Submitted,
                DateSubmitted = DateTime.UtcNow
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Claim created successfully.",
                claimId = claim.ClaimId
            });
        }

        [HttpGet("MyClaims")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyClaims()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var claims = await _context.Claims
                .Where(c => c.Policy.UserId == userId)
                .Select(c => new
                {
                    c.ClaimId,
                    c.PolicyId,
                    c.Description,
                    c.DateSubmitted,
                    c.Status,
                    productName = c.Policy.Product.Name
                })
                .ToListAsync();

            return Ok(claims);
        }


        [HttpGet("AllClaims")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllClaims()
        {
            var claims = await _context.Claims
                .OrderByDescending(c => c.DateSubmitted)
                .Select(c => new
                {
                    c.ClaimId,
                    c.PolicyId,
                    UserId = c.Policy.UserId,
                    c.Description,
                    c.DateSubmitted,
                    c.Status,
                    ProductName = c.Policy.Product.Name
                })
                .ToListAsync();

            return Ok(claims);
        }


        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClaimStatus(int id, UpdateClaimStatusDTO dto)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound("Claim not found.");
            }
            if (claim.Status == ClaimStatus.Approved ||
                claim.Status == ClaimStatus.Denied)
            {
                return BadRequest("This claim has already been finalized.");
            }

            claim.Status = dto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}