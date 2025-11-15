using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StudentPerformanceTracker.Data.Entities.AdminManagement;
using StudentPerformanceTracker.Services.Authentication;
using StudentPerformanceTracker.Services.Teachers;
using Xunit;

namespace StudentPerformanceTracker.Tests.Services
{
    public class TeacherServiceTests : TestBase
    {
        private readonly TeacherService _teacherService;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ILogger<TeacherService>> _loggerMock;

        public TeacherServiceTests()
        {
            _passwordServiceMock = new Mock<IPasswordService>();
            _loggerMock = new Mock<ILogger<TeacherService>>();
            
            _passwordServiceMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("HashedPassword123");

            _teacherService = new TeacherService(
                Context, 
                _passwordServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateTeacherAsync_ShouldCreateTeacher_WithValidData()
        {
            // Arrange
            var teacher = new TeacherManagement
            {
                Username = "testteacher",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Today.AddYears(-30),
                Age = 30,
                Address = "123 Test Street",
                IsActive = true
            };
            var password = "TestPassword123!";
            var subjectIds = new List<int>();

            // Act
            var result = await _teacherService.CreateTeacherAsync(teacher, password, subjectIds);

            // Assert
            result.Should().NotBeNull();
            result.TeacherId.Should().BeGreaterThan(0);
            result.Username.Should().Be("testteacher");
            result.PasswordHash.Should().Be("HashedPassword123");
            _passwordServiceMock.Verify(x => x.HashPassword(password), Times.Once);
        }

        [Fact]
        public async Task CreateTeacherAsync_ShouldThrowException_WhenUsernameExists()
        {
            // Arrange
            var existingTeacher = new TeacherManagement
            {
                Username = "duplicateuser",
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = DateTime.Today.AddYears(-28),
                Age = 28,
                Address = "456 Test Avenue",
                PasswordHash = "ExistingHash",
                IsActive = true
            };
            Context.Teachers.Add(existingTeacher);
            await Context.SaveChangesAsync();

            var newTeacher = new TeacherManagement
            {
                Username = "duplicateuser", // Same username
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = DateTime.Today.AddYears(-25),
                Age = 25,
                Address = "789 Test Road",
                IsActive = true
            };
            var password = "TestPassword123!";
            var subjectIds = new List<int>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _teacherService.CreateTeacherAsync(newTeacher, password, subjectIds)
            );
        }

        [Fact]
        public async Task GetAllTeachersAsync_ShouldReturnAllTeachers()
        {
            // Arrange
            var teachers = new List<TeacherManagement>
            {
                new TeacherManagement
                {
                    Username = "teacher1",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    DateOfBirth = DateTime.Today.AddYears(-30),
                    Age = 30,
                    Address = "Address 1",
                    PasswordHash = "Hash1",
                    IsActive = true
                },
                new TeacherManagement
                {
                    Username = "teacher2",
                    FirstName = "Bob",
                    LastName = "Williams",
                    DateOfBirth = DateTime.Today.AddYears(-35),
                    Age = 35,
                    Address = "Address 2",
                    PasswordHash = "Hash2",
                    IsActive = true
                }
            };
            Context.Teachers.AddRange(teachers);
            await Context.SaveChangesAsync();

            // Act
            var result = await _teacherService.GetAllTeachersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Username == "teacher1");
            result.Should().Contain(t => t.Username == "teacher2");
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnTrue_WhenTeacherExists()
        {
            // Arrange
            var teacher = new TeacherManagement
            {
                Username = "todelete",
                FirstName = "Delete",
                LastName = "Me",
                DateOfBirth = DateTime.Today.AddYears(-25),
                Age = 25,
                Address = "Delete Address",
                PasswordHash = "Hash",
                IsActive = true
            };
            Context.Teachers.Add(teacher);
            await Context.SaveChangesAsync();
            var teacherId = teacher.TeacherId;

            // Act
            var result = await _teacherService.DeleteTeacherAsync(teacherId);

            // Assert
            result.Should().BeTrue();
            var deletedTeacher = await Context.Teachers.FindAsync(teacherId);
            deletedTeacher.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTeacherAsync_ShouldReturnFalse_WhenTeacherDoesNotExist()
        {
            // Act
            var result = await _teacherService.DeleteTeacherAsync(999);

            // Assert
            result.Should().BeFalse();
        }
    }
}