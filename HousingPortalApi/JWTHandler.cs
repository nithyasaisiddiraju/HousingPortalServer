﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HousingPortalApi.Models;

namespace HousingPortalApi
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<HousingPortalUser> _userManager;
        public JwtHandler(IConfiguration configuration, UserManager<HousingPortalUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<JwtSecurityToken> GetTokenAsync(HousingPortalUser user, Claim[] additionalClaims = null) =>
            new(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: await GetClaimsAsync(user, additionalClaims),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"])),
                signingCredentials: GetSigningCredentials());

        private SigningCredentials GetSigningCredentials()
        {
            byte[] key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]!);
            SymmetricSecurityKey secret = new(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(HousingPortalUser user, Claim[] additionalClaims)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            claims.AddRange(from role in await _userManager.GetRolesAsync(user) select new Claim(ClaimTypes.Role, role));
            return claims;
        }
    }
 }
