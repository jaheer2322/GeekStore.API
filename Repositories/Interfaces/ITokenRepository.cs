using Microsoft.AspNetCore.Identity;

namespace GeekStore.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);

    }
}
