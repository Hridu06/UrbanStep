using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanStep.DTOs.Auth;
using UrbanStep.Helpers;
using UrbanStep.Models;

namespace UrbanStep.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(new { message = "Email is already registered." });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            await _userManager.AddToRoleAsync(user, "Customer");

            return await BuildAuthResponse(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { message = "Invalid email or password." });

            return await BuildAuthResponse(user);
        }

        [HttpPost("admin-login")]
        public async Task<ActionResult<AuthResponseDto>> AdminLogin(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { message = "Invalid email or password." });

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return StatusCode(403, new { message = "This account does not have admin access." });

            return await BuildAuthResponse(user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AuthResponseDto>> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userId == null ? null : await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            return await BuildAuthResponse(user);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<AuthResponseDto>> UpdateProfile(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userId == null ? null : await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            return await BuildAuthResponse(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("promote-to-admin")]
        public async Task<ActionResult<AuthResponseDto>> PromoteToAdmin(PromoteToAdminDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new { message = "No account found with that email." });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest(new { message = "This account is already an admin." });

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, "Admin");

            return await BuildAuthResponse(user);
        }

        private async Task<AuthResponseDto> BuildAuthResponse(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = JwtHelper.GenerateToken(user, roles, _config, out var expiration);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "Customer"
            };
        }
    }
}
