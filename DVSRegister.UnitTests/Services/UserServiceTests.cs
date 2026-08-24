using AutoMapper;
using DVSRegister.BusinessLogic;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())).CreateMapper();
            _service = new UserService(_userRepository, mapper);
        }

        private static CabUser CreateCabUser(AccountStatusEnum status = AccountStatusEnum.Active, Cab? cab = null)
        {
            return new CabUser
            {
                Id = 10,
                CabId = 20,
                CabEmail = "user@example.com",
                UserName = "test-user",
                FullName = "Test User",
                AccountStatus = status,
                CreatedTime = new DateTime(2024, 1, 1),
                ModifiedDate = new DateTime(2024, 2, 1),
                LastLoggedIn = new DateTime(2024, 3, 1),
                Cab = cab
            };
        }

        #region GetUser

        [Fact]
        public async Task GetUser_ExistingUser_ReturnsMappedUserWithLinkedCab()
        {
            var cab = new Cab { Id = 20, CabName = "Example CAB" };
            var user = CreateCabUser(cab: cab);
            _userRepository.GetUser("user@example.com").Returns(user);

            var result = await _service.GetUser("user@example.com");

            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.CabId, result.CabId);
            Assert.Equal(user.CabEmail, result.CabEmail);
            Assert.Equal(user.UserName, result.UserName);
            Assert.Equal(user.FullName, result.FullName);
            Assert.Equal(user.AccountStatus, result.AccountStatus);
            Assert.Equal(user.CreatedTime, result.CreatedTime);
            Assert.Equal(user.ModifiedDate, result.ModifiedDate);
            Assert.Equal(user.LastLoggedIn, result.LastLoggedIn);
            Assert.NotNull(result.Cab);
            Assert.Equal(cab.Id, result.Cab.Id);
            Assert.Equal(cab.CabName, result.Cab.CabName);
            await _userRepository.Received(1).GetUser("user@example.com");
        }

        [Fact]
        public async Task GetUser_DefaultUserRecord_ReturnsMappedDefaultUser()
        {
            _userRepository.GetUser("missing@example.com").Returns(new CabUser());

            var result = await _service.GetUser("missing@example.com");

            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Equal(0, result.CabId);
            Assert.Null(result.CabEmail);
            Assert.Null(result.Cab);
        }

        [Fact]
        public async Task GetUser_SuspendedUser_ReturnsMappedSuspendedUser()
        {
            var user = CreateCabUser(AccountStatusEnum.Suspended);
            _userRepository.GetUser(user.CabEmail).Returns(user);

            var result = await _service.GetUser(user.CabEmail);

            Assert.Equal(AccountStatusEnum.Suspended, result.AccountStatus);
            Assert.Equal(user.CabEmail, result.CabEmail);
        }

        [Fact]
        public async Task GetUser_NullLinkedCab_ReturnsMappedUserWithNullCab()
        {
            var user = CreateCabUser(cab: null);
            _userRepository.GetUser(user.CabEmail).Returns(user);

            var result = await _service.GetUser(user.CabEmail);

            Assert.NotNull(result);
            Assert.Null(result.Cab);
        }

        [Fact]
        public async Task GetUser_NullRepositoryResult_ReturnsNull()
        {
            _userRepository.GetUser("missing@example.com").Returns((CabUser)null!);

            var result = await _service.GetUser("missing@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUser_RepositoryThrows_PropagatesException()
        {
            _userRepository.GetUser(Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetUser("user@example.com"));
        }

        #endregion

        #region UpdateCabUser

        [Fact]
        public async Task UpdateCabUser_ExistingUser_ReturnsMappedUpdatedUser()
        {
            var lastLoggedIn = new DateTime(2025, 4, 1);
            var user = CreateCabUser();
            user.LastLoggedIn = lastLoggedIn;
            _userRepository.UpdateCabUser(user.CabEmail).Returns(user);

            var result = await _service.UpdateCabUser(user.CabEmail);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.CabEmail, result.CabEmail);
            Assert.Equal(lastLoggedIn, result.LastLoggedIn);
            await _userRepository.Received(1).UpdateCabUser(user.CabEmail);
        }

        [Fact]
        public async Task UpdateCabUser_NullRepositoryResult_ReturnsNull()
        {
            _userRepository.UpdateCabUser("missing@example.com").Returns((CabUser)null!);

            var result = await _service.UpdateCabUser("missing@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCabUser_RepositoryThrows_PropagatesException()
        {
            _userRepository.UpdateCabUser(Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateCabUser("user@example.com"));
        }

        #endregion

        #region GetDSITUserEmails

        [Fact]
        public async Task GetDSITUserEmails_MultipleEmails_ReturnsRepositoryList()
        {
            var emails = new List<string> { "manager@example.com", "admin@example.com" };
            _userRepository.GetDSITUserEmails().Returns(emails);

            var result = await _service.GetDSITUserEmails();

            Assert.Same(emails, result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetDSITUserEmails_EmptyResultSet_ReturnsEmptyList()
        {
            var emails = new List<string>();
            _userRepository.GetDSITUserEmails().Returns(emails);

            var result = await _service.GetDSITUserEmails();

            Assert.Same(emails, result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDSITUserEmails_NullRepositoryResult_ReturnsNull()
        {
            _userRepository.GetDSITUserEmails().Returns(Task.FromResult<List<string>>(null!));

            var result = await _service.GetDSITUserEmails();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDSITUserEmails_RepositoryThrows_PropagatesException()
        {
            _userRepository.GetDSITUserEmails()
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetDSITUserEmails());
        }

        #endregion
    }
}
