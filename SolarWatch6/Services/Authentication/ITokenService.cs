using Microsoft.AspNetCore.Identity;

namespace SolarWatch6.Services.Authentication
{
    public interface ITokenService
    {
        public string CreateToken(IdentityUser user);
    }
}
