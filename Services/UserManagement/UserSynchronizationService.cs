using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Services.Interfaces.UserManagement
{
    public class UserSynchronizationService : IUserSynchronizationService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserSynchronizationService> _logger;

        public UserSynchronizationService(AppDbContext dbContext, ILogger<UserSynchronizationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SyncEmployeesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting user synchronization from employees...");

            var employeeRoleId = await _dbContext.RoleMasters
                .Where(r => r.Name == "Employee" && !r.IsDeleted)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (employeeRoleId == Guid.Empty)
            {
                _logger.LogError("Employee role not found in RoleMasters table. Sync aborted.");
                return;
            }

            var employees = await _dbContext.Employees
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(e => e.EmployeeStatus)
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);

            var existingUsers = await _dbContext.Users
                .IgnoreQueryFilters()
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync(cancellationToken);

            var usersByEmail = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
            var usersByUserName = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in existingUsers)
            {
                if (!string.IsNullOrWhiteSpace(u.Email) && !usersByEmail.ContainsKey(u.Email))
                    usersByEmail[u.Email] = u;

                if (!string.IsNullOrWhiteSpace(u.UserName) && !usersByUserName.ContainsKey(u.UserName))
                    usersByUserName[u.UserName] = u;
            }

            var usersToCreate = new List<User>();
            var usersToUpdate = new List<User>();

            foreach (var employee in employees)
            {
                if (string.IsNullOrWhiteSpace(employee.EmployeeCode)) continue;

                var matchedUser = FindMatchingUser(employee, existingUsers, usersByEmail, usersByUserName);

                if (matchedUser != null)
                {
                    if (matchedUser.Role != null && matchedUser.Role.Name == "Admin") continue;

                    var hasChanges = UpdateUserFromEmployee(matchedUser, employee);

                    if (matchedUser.EmployeeId != employee.Id)
                    {
                        matchedUser.EmployeeId = employee.Id;
                        hasChanges = true;
                    }

                    if (hasChanges)
                    {
                        matchedUser.UpdatedAt = DateTime.UtcNow;
                        usersToUpdate.Add(matchedUser);
                    }
                }
                else
                {
                    var newUser = CreateUserFromEmployee(employee, usersToCreate, employeeRoleId);
                    usersToCreate.Add(newUser);
                }
            }

            if (usersToCreate.Any())
            {
                _dbContext.Users.AddRange(usersToCreate);
                _logger.LogInformation("Creating {Count} new users from employees", usersToCreate.Count);
            }

            if (usersToUpdate.Any())
            {
                _dbContext.Users.UpdateRange(usersToUpdate);
                _logger.LogInformation("Updating {Count} existing users from employees", usersToUpdate.Count);
            }

            if (usersToCreate.Any() || usersToUpdate.Any())
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("User synchronization completed. Created: {Created}, Updated: {Updated}",
                usersToCreate.Count, usersToUpdate.Count);
        }

        private static User? FindMatchingUser(
            Employee employee,
            List<User> existingUsers,
            Dictionary<string, User> usersByEmail,
            Dictionary<string, User> usersByUserName)
        {
            var matchedUser = existingUsers.FirstOrDefault(u => u.EmployeeId == employee.Id);

            if (matchedUser == null && !string.IsNullOrWhiteSpace(employee.Email))
            {
                usersByUserName.TryGetValue(employee.Email, out matchedUser);
            }

            if (matchedUser == null && !string.IsNullOrWhiteSpace(employee.Email))
            {
                usersByEmail.TryGetValue(employee.Email, out matchedUser);
            }

            return matchedUser;
        }

        private bool UpdateUserFromEmployee(User user, Employee employee)
        {
            var hasChanges = false;

            if (user.Name != employee.FullName)
            {
                user.Name = employee.FullName;
                hasChanges = true;
            }

            if (user.Email != employee.Email)
            {
                user.Email = employee.Email;
                hasChanges = true;
            }

            var desiredUserName = employee.Email;
            if (!string.IsNullOrWhiteSpace(desiredUserName) && user.UserName != desiredUserName)
            {
                user.UserName = desiredUserName;
                hasChanges = true;
            }

            var shouldBeActive = IsEmployeeActive(employee);
            if (user.IsActive != shouldBeActive)
            {
                user.IsActive = shouldBeActive;
                hasChanges = true;
            }

            return hasChanges;
        }

        private User CreateUserFromEmployee(Employee employee, List<User> pendingUsers, Guid employeeRoleId)
        {
            var userName = employee.Email;
            var suffix = 1;
            while (_dbContext.Users.IgnoreQueryFilters().Any(u => u.UserName == userName && !u.IsDeleted)
                || pendingUsers.Any(u => u.UserName == userName))
            {
                userName = $"{employee.Email}_{suffix}";
                suffix++;
            }

            var password = GenerateRandomPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            return new User
            {
                Name = employee.FullName,
                UserName = userName,
                Email = employee.Email,
                PasswordHash = passwordHash,
                RoleId = employeeRoleId,
                EmployeeId = employee.Id,
                IsActive = IsEmployeeActive(employee),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
        }

        private static bool IsEmployeeActive(Employee employee)
        {
            return employee.EmployeeStatus?.Name?.Equals("Active", StringComparison.OrdinalIgnoreCase) == true
                && !employee.IsDeleted;
        }

        private static string GenerateRandomPassword()
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()_-+=<>?";

            var random = new Random();
            var password = new char[12];

            password[0] = upper[random.Next(upper.Length)];
            password[1] = lower[random.Next(lower.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = special[random.Next(special.Length)];

            var allChars = upper + lower + digits + special;
            for (int i = 4; i < password.Length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }
    }
}
