using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using InsuranceAPI.Data;
using InsuranceAPI.Models;
using InsuranceAPI.DTOs;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceProductController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceProductController(InsuranceDbContext context)
        {
            _context = context;
        }

        // GET: api/InsuranceProduct
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsuranceProduct>>> GetInsuranceProducts()
        {
            return await _context.InsuranceProducts.ToListAsync();
        }

        // GET: api/InsuranceProduct/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceProduct>> GetInsuranceProduct(int id)
        {
            var insuranceProduct = await _context.InsuranceProducts.FindAsync(id);

            if (insuranceProduct == null)
            {
                return NotFound();
            }

            return insuranceProduct;
        }

        // CREATE: api/InsuranceProduct
        [Authorize(Roles = "Admin")] // Only Admins can create insurance products
        [HttpPost]
        public async Task<ActionResult<InsuranceProduct>> PostInsuranceProduct(CreateInsuranceProductDTO createInsuranceProductDTO)
        {
            var insuranceProduct = new InsuranceProduct
            {
                Name = createInsuranceProductDTO.Name,
                Description = createInsuranceProductDTO.Description,
                BasePremium = createInsuranceProductDTO.BasePremium
            };

            _context.InsuranceProducts.Add(insuranceProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInsuranceProduct), new { id = insuranceProduct.ProductId }, insuranceProduct);
        }
        //Update: api/InsuranceProduct/5
        [Authorize(Roles = "Admin")] // Only Admins can update insurance products
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInsuranceProduct(int id, InsuranceProduct insuranceProduct)
        {
            var existingProduct = await _context.InsuranceProducts.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = insuranceProduct.Name;
            existingProduct.Description = insuranceProduct.Description;
            existingProduct.BasePremium = insuranceProduct.BasePremium;
            existingProduct.IsActive = insuranceProduct.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        //Deactivate: api/InsuranceProduct/5
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateInsuranceProduct(int id)
        {
            var existingProduct = await _context.InsuranceProducts.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateInsuranceProduct(int id)
        {
            var existingProduct = await _context.InsuranceProducts.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.IsActive = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}