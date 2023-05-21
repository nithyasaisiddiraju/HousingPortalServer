using HousingPortalApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace HousingPortalApi.Interfaces
{
    public interface IJwtHandler
    {
        Task<JwtSecurityToken> GetTokenAsync(HousingPortalUser user, Claim[] claims);
    }
}