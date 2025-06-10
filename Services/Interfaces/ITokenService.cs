using Microsoft.AspNetCore.Identity;

namespace GeekStore.API.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);

    }
}
