using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService:ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
        _key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims=new List<Claim>
            {new Claim(JwtRegisteredClaimNames.NameId,user.Username)};
            var kreds=new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);
            var TokenDescriptor=new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(14),
                SigningCredentials=kreds
            };
           var TokenHandler=new JwtSecurityTokenHandler();
           var token=TokenHandler.CreateToken(TokenDescriptor);
           return TokenHandler.WriteToken(token);
        }
    }
}