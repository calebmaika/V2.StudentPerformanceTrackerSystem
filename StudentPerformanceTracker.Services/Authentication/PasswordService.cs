namespace StudentPerformanceTracker.Services.Authentication
{
    /// <summary>
    /// Interface for password operations
    /// Using an interface allows for easier testing and flexibility
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>BCrypt hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies if a plain text password matches a BCrypt hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="passwordHash">BCrypt hash to compare against</param>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string password, string passwordHash);
    }

    /// <summary>
    /// Service that handles password hashing and verification using BCrypt
    /// BCrypt is a secure one-way hashing algorithm designed for passwords
    /// </summary>
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// Each time you hash the same password, you get a different hash (salt is built-in)
        /// Example: HashPassword("Admin@123") might produce "$2a$11$abc123..."
        /// </summary>
        public string HashPassword(string password)
        {
            // BCrypt automatically generates a salt and includes it in the hash
            // The default work factor is 11 (2^11 iterations - secure but not too slow)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies if a password matches a hash
        /// BCrypt extracts the salt from the hash and compares
        /// Example: VerifyPassword("Admin@123", "$2a$11$abc123...") returns true/false
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                // Returns true if password matches the hash, false otherwise
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                // If hash is invalid or corrupted, return false
                return false;
            }
        }
    }
}