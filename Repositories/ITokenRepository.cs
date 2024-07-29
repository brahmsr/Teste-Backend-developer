using Microsoft.AspNetCore.Identity;

namespace APItesteInside.Repositories
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}
