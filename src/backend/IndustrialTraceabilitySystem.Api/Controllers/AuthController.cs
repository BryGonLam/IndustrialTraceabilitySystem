using System.Security.Claims;
using IndustrialTraceabilitySystem.Api.Application.Dtos.Auth;
using IndustrialTraceabilitySystem.Api.Infrastructure.Auth;
using IndustrialTraceabilitySystem.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndustrialTraceabilitySystem.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<Domain.Entities.User> _passwordHasher;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = new PasswordHasher<Domain.Entities.User>();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = new { code = "INVALID_CREDENTIALS", message = "Username and password are required." } });
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            return Unauthorized(new { error = new { code = "INVALID_CREDENTIALS", message = "Invalid username or password." } });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { error = new { code = "USER_INACTIVE", message = "This user is inactive." } });
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { error = new { code = "INVALID_CREDENTIALS", message = "Invalid username or password." } });
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        var response = new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserInfoResponse
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Roles = roles
            }
        };

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfoResponse>> Me()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (sub is null || !int.TryParse(sub, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "Token does not contain a valid user id." } });
        }

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            return Unauthorized(new { error = new { code = "USER_NOT_FOUND", message = "User no longer exists." } });
        }

        var response = new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(response);
    }
}