using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PD421_Dashboard_WEB_API.BLL.Dtos.Register;
using PD421_Dashboard_WEB_API.BLL.Settings;
using PD421_Dashboard_WEB_API.DAL.Entitites.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public RegisterService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Picture, user.Image ?? string.Empty),
                new Claim("firstName", user.FirstName ?? string.Empty),
                new Claim("lastName", user.LastName ?? string.Empty)
            };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                var roleClaims = roles.Select(role => new Claim("roles", role));
                claims.AddRange(roleClaims);
            }
            string secretKey = _jwtSettings.SecretKey;
            var registerKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var regiterCredentials = new SigningCredentials(registerKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: regiterCredentials,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ServiceResponse> RegisterAsync(RegisterDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);
            if(user != null)
            {
                return new ServiceResponse
                {
                    Message = "Користувач з таким логіном або поштою вже існує",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }
            var createUser = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };
            await _userManager.CreateAsync(createUser, dto.Password);

            var token = await GenerateJwtToken(createUser);
            return new ServiceResponse
            {
                Message = "Успішно зареєстровано",
                IsSuccess = true,
                HttpStatusCode = HttpStatusCode.OK,
                Data = token,
            };
        }
    }
}
