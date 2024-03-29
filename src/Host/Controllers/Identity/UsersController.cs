using EcoMotorsPractice.Application.Identity.Users;
using EcoMotorsPractice.Application.Identity.Users.Password;

namespace EcoMotorsPractice.Host.Controllers.Identity;

public class UsersController : VersionNeutralApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    [OpenApiOperation("Get list of all users.", "")]
    public Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return _userService.GetListAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    [OpenApiOperation("Get a user's details.", "")]
    public Task<UserDetailsDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetAsync(id, cancellationToken);
    }

    [HttpGet("{id}/roles")]
    [Authorize(Roles = AppRoles.Admin)]
    [OpenApiOperation("Get a user's roles.", "")]
    public Task<List<UserRoleDto>> GetRolesAsync(string id, CancellationToken cancellationToken)
    {
        return _userService.GetRolesAsync(id, cancellationToken);
    }

    [HttpPost("{id}/roles")]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    [Authorize(Roles = AppRoles.Admin)]
    [OpenApiOperation("Update a user's assigned roles.", "")]
    public Task<string> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
    {
        return _userService.AssignRolesAsync(id, request, cancellationToken);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [OpenApiOperation("Creates a new user.", "")]
    public Task<string> CreateAsync(CreateUserRequest request)
    {
        // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
        // and return UnAuthorized when it isn't
        // Also: add other protection to prevent automatic posting (captcha?)
        return _userService.CreateAsync(request, GetOriginFromRequest());
    }

    [HttpPost("self-register")]
    [AllowAnonymous]
    [OpenApiOperation("Anonymous user creates a user.", "")]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    public Task<string> SelfRegisterAsync(CreateUserRequest request)
    {
        // TODO: check if registering anonymous users is actually allowed (should probably be an appsetting)
        // and return UnAuthorized when it isn't
        // Also: add other protection to prevent automatic posting (captcha?)
        return _userService.CreateAsync(request, GetOriginFromRequest());
    }

    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = AppRoles.Admin)]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return BadRequest();
        }

        await _userService.ToggleStatusAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for a user.", "")]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Search))]
    public Task<string> ConfirmEmailAsync([FromQuery] string tenant, [FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
    {
        return _userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken);
    }

    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm phone number for a user.", "")]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Search))]
    public Task<string> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return _userService.ConfirmPhoneNumberAsync(userId, code);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [OpenApiOperation("Request a pasword reset email for a user.", "")]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    public Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return _userService.ForgotPasswordAsync(request, GetOriginFromRequest());
    }

    [HttpPost("reset-password")]
    [OpenApiOperation("Reset a user's password.", "")]
    [AllowAnonymous]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    public Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return _userService.ResetPasswordAsync(request);
    }

    [HttpPost("verify-otp-token")]
    [OpenApiOperation("Verify user's otp token.", "")]
    [AllowAnonymous]
    [ApiConventionMethod(typeof(AppApiConventions), nameof(AppApiConventions.Register))]
    public Task<bool> VerifyOtpTokenAsync(VerifyOtpTokenRequest request)
    {
        return _userService.VerifyOtpTokenAsync(request);
    }

    private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
}