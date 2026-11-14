using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;
using SpaRes.Api.Dtos;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SpaResDbContext _db;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AuthController(SpaResDbContext db,IPasswordHasher<AppUser> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var staffId = User.FindFirst("StaffId")?.Value;
            var tenantId = User.FindFirst("TenantId")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                StaffId = staffId,
                TenantId = tenantId,
                Role = role
            });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto request)
        {
            var staff = _db.Staff
                .FirstOrDefault(s =>
                    s.StaffId == request.StaffId &&
                    s.TenantId == request.TenantId);

            if (staff == null)
                return BadRequest("Staff member not found.");

            var existingUser = _db.AppUsers
                .FirstOrDefault(u => u.Email == request.Email);

            if (existingUser != null)
                return BadRequest("An account with this email already exists.");

            var user = new AppUser
            {
                TenantId = request.TenantId,
                StaffId = request.StaffId,
                Email = request.Email,
                Role = request.Role,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password
            );

            _db.AppUsers.Add(user);
            _db.SaveChanges();

            return Ok(new
            {
                user.AppUserId,
                user.TenantId,
                user.StaffId,
                user.Email,
                user.Role,
                user.IsActive
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto request)
        {
            var user = _db.AppUsers
                .FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            if (!user.IsActive)
                return Unauthorized("This account is inactive.");

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid email or password.");

            var claims = new[]
            {
                new Claim("AppUserId", user.AppUserId.ToString()),
                new Claim("StaffId", user.StaffId.ToString()),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "SpaRes-Development-Key-Change-Me-2026-123456789"
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: "SpaRes.Api",
                audience: "SpaRes.Web",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            //Result is: xxxxx.yyyyy.zzzzz

            return Ok(new
            {
                user.AppUserId,
                user.TenantId,
                user.StaffId,
                user.Email,
                user.Role,
                tokenString
            });
        }
    }
}