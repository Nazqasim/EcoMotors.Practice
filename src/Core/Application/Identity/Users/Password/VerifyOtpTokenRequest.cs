namespace EcoMotorsPractice.Application.Identity.Users.Password;

public class VerifyOtpTokenRequest
{
    public string? Email { get; set; }
    public string? Token { get; set; }
}