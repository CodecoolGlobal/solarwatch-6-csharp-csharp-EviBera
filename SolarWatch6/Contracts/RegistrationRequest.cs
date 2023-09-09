using System.ComponentModel.DataAnnotations;

namespace SolarWatch6.Contracts
{
    public record RegistrationRequest(
    [Required] string Email,
    [Required] string Username,
    [Required] string Password);
}
