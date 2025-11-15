using FluentAssertions;
using StudentPerformanceTracker.Services.Authentication;
using Xunit;

namespace StudentPerformanceTracker.Tests.Services
{
    public class PasswordServiceTests
    {
        private readonly IPasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ShouldReturnNonEmptyString()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password); // Hash should be different from original
        }

        [Fact]
        public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _passwordService.HashPassword(password);
            var hash2 = _passwordService.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2); // BCrypt includes random salt
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatches()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(password, hashedPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var password = "TestPassword123!";
            var wrongPassword = "WrongPassword456!";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenHashIsInvalid()
        {
            // Arrange
            var password = "TestPassword123!";
            var invalidHash = "InvalidHashString";

            // Act
            var result = _passwordService.VerifyPassword(password, invalidHash);

            // Assert
            result.Should().BeFalse();
        }
    }
}