using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
            _logger = logger;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _authRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return null;
                }

                var token = BuildJwtToken(user);
                var refreshToken = GenerateRefreshTokenString();

                var rtEntity = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

// persist refresh token using unit of work
await _unitOfWork.RefreshTokens.AddAsync(rtEntity, cancellationToken);
await _unitOfWork.SaveAsync(cancellationToken);

                return new LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        Role = user.Role,
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while authenticating user {Email}", request?.Email);
                throw;
            }
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
                if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow)
                    return null;

                var user = await _authRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
                if (user is null || !user.IsActive)
                    return null;

                await _unitOfWork.RefreshTokens.DeleteAsync(storedToken, cancellationToken);

                var newAccessToken = BuildJwtToken(user);
                var newRefreshToken = GenerateRefreshTokenString();

                var rtEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

                await _unitOfWork.RefreshTokens.AddAsync(rtEntity, cancellationToken);

                return new LoginResponse
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        Role = user.Role,
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while refreshing token");
                throw;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _authRepository.GetByIdAsync(userId, cancellationToken);
                if (user is null)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching current user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
                if (storedToken is null)
                    return false;

                await _unitOfWork.RefreshTokens.DeleteAsync(storedToken, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while revoking refresh token");
                throw;
            }
        }

        private string BuildJwtToken(Models.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var keyBytes = Convert.FromBase64String(_jwtSettings.Key);
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshTokenString()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
