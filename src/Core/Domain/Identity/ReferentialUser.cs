using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoMotorsPractice.Domain.Identity;
public class ReferentialUser : BaseEntity<string>, IAggregateRoot
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string UserName { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? DeviceToken { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? ObjectId { get; set; }
}
