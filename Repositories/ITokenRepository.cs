using Microsoft.AspNetCore.Identity;

namespace GeekStore.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
      
    }
}
