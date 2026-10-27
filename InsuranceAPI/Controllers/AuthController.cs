using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using InsuranceAPI.Data;
using InsuranceAPI.Models;
using InsuranceAPI.DTOs;
using JwtClaim = System.Security.Claims.Claim;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(InsuranceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            // Check if the email is already registered
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                //409 conflicts with existing resource
                return Conflict("Email is already registered.");
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = RoleType.Customer,
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //we return a 201 status without exposing sensitive user information
            //such as the password hash. Instead, we return a message and the user ID.
            return StatusCode(201, new
            {
                message = "User registered successfully.",
                userId = user.UserId
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            //allows us to verify the hashed password against the provided password
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword
            (
                user,
                user.PasswordHash,
                dto.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                // cspell:ignore Hmac
                SecurityAlgorithms.HmacSha256
            );
            var claims = new[]
            {
                new JwtClaim(
                ClaimTypes.NameIdentifier, user.UserId.ToString()),

                new JwtClaim(
                ClaimTypes.Email, user.Email),

                new JwtClaim(
                ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpireMinutes"]!)
                ),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString
            });
        }

        [Authorize]
        [HttpGet("Profile")]
        public IActionResult GetProfile()
        {
            

            return Ok(new
            {
                message = "You accessed a protected endpoint.",
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = User.FindFirstValue(ClaimTypes.Email)
            });
        }
    }
}