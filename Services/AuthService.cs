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

        public const string DefaultPassword = "NV@12345#";

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _authRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return null;
                }

                var forcePasswordChange = user.PasswordResetRequired;
                var token = BuildJwtToken(user, forcePasswordChange);
                var refreshToken = GenerateRefreshTokenString();

                var rtEntity = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

                await _unitOfWork.RefreshTokens.AddAsync(rtEntity, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return new LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ForcePasswordChange = forcePasswordChange,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.Name ?? string.Empty,
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

                var forcePasswordChange = user.PasswordResetRequired;
                var newAccessToken = BuildJwtToken(user, forcePasswordChange);
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
                    ForcePasswordChange = forcePasswordChange,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.Name ?? string.Empty,
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
                    RoleId = user.RoleId,
                    RoleName = user.Role?.Name ?? string.Empty,
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

        public async Task<ChangePasswordResponse?> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _authRepository.GetByIdAsync(userId, cancellationToken);
                if (user is null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                    return null;

                if (request.NewPassword != request.ConfirmPassword)
                    return null;

                if (request.NewPassword.Length < 8 ||
                    !request.NewPassword.Any(char.IsUpper) ||
                    !request.NewPassword.Any(char.IsLower) ||
                    !request.NewPassword.Any(char.IsDigit) ||
                    !request.NewPassword.Any(c => !char.IsLetterOrDigit(c)))
                    return null;

                if (request.NewPassword == DefaultPassword)
                    return null;

                if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
                    return null;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.IsFirstLogin = false;
                user.IsDefaultPassword = false;
                user.PasswordResetRequired = false;
                user.PasswordChangedOn = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _authRepository.UpdateAsync(user, cancellationToken);

                await _unitOfWork.RefreshTokens.DeleteAllForUserAsync(userId, cancellationToken);

                _logger.LogInformation("Password changed successfully for user {UserId}. All sessions invalidated.", userId);

                return new ChangePasswordResponse
                {
                    Success = true,
                    Message = "Password changed successfully. Please login again."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while changing password for user {UserId}", userId);
                return null;
            }
        }

        private string BuildJwtToken(Models.User user, bool forcePasswordChange = false)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
                new Claim("passwordResetRequired", forcePasswordChange ? "true" : "false"),
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
