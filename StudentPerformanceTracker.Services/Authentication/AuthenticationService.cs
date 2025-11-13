using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentPerformanceTracker.Services.Authentication
{
    /// <summary>
    /// Interface for authentication operations
    /// </summary>
    public interface IAdminAuthenticationService
    {
        /// <summary>
        /// Authenticates an admin user with username and password
        /// </summary>
        /// <param name="username">Username to authenticate</param>
        /// <param name="password">Plain text password</param>
        /// <returns>Admin object if authentication succeeds, null if it fails</returns>
        Task<Admin?> AuthenticateAdminAsync(string username, string password);

        /// <summary>
        /// Updates the last login timestamp for an admin
        /// </summary>
        /// <param name="adminId">ID of the admin who just logged in</param>
        Task UpdateLastLoginAsync(int adminId);
    }

    /// <summary>
    /// Service that handles admin authentication logic
    /// This is where the actual login validation happens
    /// </summary>
    public class AuthenticationService : IAdminAuthenticationService

    {
        // Database context to query the Admins table
        private readonly ApplicationDbContext _context;
        
        // Password service to verify password hashes
        private readonly IPasswordService _passwordService;

        /// <summary>
        /// Constructor - ASP.NET Core will automatically inject these dependencies
        /// This is called "Dependency Injection"
        /// </summary>
        public AuthenticationService(
            ApplicationDbContext context, 
            IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Authenticates an admin by checking username and password
        /// Flow:
        /// 1. Find admin by username
        /// 2. Check if admin exists and is active
        /// 3. Verify password matches the stored hash
        /// 4. Return admin if successful, null if failed
        /// </summary>
        public async Task<Admin?> AuthenticateAdminAsync(string username, string password)
        {
            // Step 1: Find admin by username and check if active
            // FirstOrDefaultAsync returns null if not found
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);

            // Step 2: If admin not found, return null (authentication failed)
            if (admin == null)
            {
                return null;
            }

            // Step 3: Verify the password against the stored hash
            // This uses BCrypt to securely compare the plain password with the hash
            bool isPasswordValid = _passwordService.VerifyPassword(password, admin.PasswordHash);

            // Step 4: If password doesn't match, return null (authentication failed)
            if (!isPasswordValid)
            {
                return null;
            }

            // Step 5: Authentication succeeded! Return the admin object
            return admin;
        }

        /// <summary>
        /// Updates the LastLoginAt timestamp when admin successfully logs in
        /// This helps track when admins last accessed the system
        /// </summary>
        public async Task UpdateLastLoginAsync(int adminId)
        {
            // Find the admin by ID
            var admin = await _context.Admins.FindAsync(adminId);
            
            if (admin != null)
            {
                // Update the last login time to now (UTC time to avoid timezone issues)
                admin.LastLoginAt = DateTime.UtcNow;
                
                // Save changes to database
                await _context.SaveChangesAsync();
            }
        }
    }
}