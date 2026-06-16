using System;
using System.Collections.Generic;
using System.Linq;

namespace HRMS.Api
{
    /// <summary>
    /// Custom attribute to mark controllers or action methods for custom authentication and role-based authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute
    {
        private string _roles = string.Empty;

        /// <summary>
        /// Gets or sets a comma-separated list of roles allowed to access the resource.
        /// Guaranteed to never return null.
        /// </summary>
        public string Roles
        {
            get => _roles;
            set => _roles = value ?? string.Empty;
        }

        /// <summary>
        /// Gets a clean, pre-parsed list of roles.
        /// If no roles are specified, this list will be empty (not null).
        /// </summary>
        public IReadOnlyList<string> AllowedRolesList { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// Restricts access to authenticated users only.
        /// </summary>
        public CustomAuthorizeAttribute()
        {
            Roles = string.Empty;
            AllowedRolesList = Array.Empty<string>();
        }

        /// <summary>
        /// Restricts access to authenticated users belonging to specific roles.
        /// </summary>
        /// <param name="roles">Comma-separated list of allowed roles.</param>
        public CustomAuthorizeAttribute(string roles)
        {
            Roles = roles;

            // Pre-parse the roles string safely during initialization
            if (!string.IsNullOrWhiteSpace(roles))
            {
                AllowedRolesList = roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList()
                    .AsReadOnly();
            }
        }
    }
}
