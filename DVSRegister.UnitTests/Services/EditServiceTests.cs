using AutoMapper;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services.Edit;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Edit;
using DVSRegister.Data.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class EditServiceTests
    {
        private readonly IEditRepository _editRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EditService> _logger;
        private readonly IProviderEditEmailSender _emailSender;
        private readonly EditService _service;

        public EditServiceTests()
        {
            _editRepository = Substitute.For<IEditRepository>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<EditService>>();
            _emailSender = Substitute.For<IProviderEditEmailSender>();

            _service = new EditService(
                _editRepository,
                _mapper,
                _emailSender,
                _logger);
        }

        [Fact]
        public async Task GetProviderDetails_ValidProvider_ReturnsMappedProvider()
        {
            // Arrange
            var providerId = 1;
            var cabId = 2;

            var provider = new ProviderProfile();
            var mappedProvider = new ProviderProfileDto();

            _editRepository
                .GetProviderDetails(providerId, cabId)
                .Returns(provider);

            _mapper
                .Map<ProviderProfileDto>(provider)
                .Returns(mappedProvider);

            // Act
            var result = await _service.GetProviderDetails(providerId, cabId);

            // Assert
            Assert.Same(mappedProvider, result);
        }

        [Fact]
        public async Task GetProviderDetails_ValidProvider_CallsRepositoryWithCorrectIds()
        {
            // Arrange
            var providerId = 1;
            var cabId = 2;

            var provider = new ProviderProfile();
            var mappedProvider = new ProviderProfileDto();

            _editRepository
                .GetProviderDetails(providerId, cabId)
                .Returns(provider);

            _mapper
                .Map<ProviderProfileDto>(provider)
                .Returns(mappedProvider);

            // Act
            await _service.GetProviderDetails(providerId, cabId);

            // Assert
            await _editRepository
                .Received(1)
                .GetProviderDetails(providerId, cabId);
        }

        [Fact]
        public async Task GetProviderDetails_RepositoryThrows_PropagatesException()
        {
            // Arrange
            var providerId = 1;
            var cabId = 2;

            _editRepository
                .GetProviderDetails(providerId, cabId)
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.GetProviderDetails(providerId, cabId));

            // Assert
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task SaveProviderDraft_UnsuccessfulSave_DoesNotSendEmails()
        {
            // Arrange
            var draftDto = new ProviderProfileDraftDto
            {
                ProviderProfileId = 1
            };

            var draftEntity = new ProviderProfileDraft();

            var response = new GenericResponse
            {
                Success = false
            };

            _mapper
                .Map<ProviderProfileDraft>(draftDto)
                .Returns(draftEntity);

            _editRepository
                .SaveProviderDraft(draftEntity, "user@test.com")
                .Returns(response);

            // Act
            var result = await _service.SaveProviderDraft(
                draftDto,
                "user@test.com",
                2);

            // Assert
            Assert.Same(response, result);

            await _emailSender
                .DidNotReceive()
                .SendProviderEditRequestSubmittedToCab(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>());

            await _emailSender
                .DidNotReceive()
                .SendProviderEditRequestSubmittedToOfdia(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>());
        }

        [Fact]
        public async Task SaveProviderDraft_SuccessfulSave_SendsBothEmails()
        {
            // Arrange
            var draftDto = new ProviderProfileDraftDto
            {
                ProviderProfileId = 1,
                RegisteredName = "New Provider Name"
            };

            var draftEntity = new ProviderProfileDraft();

            var response = new GenericResponse
            {
                Success = true
            };

            var currentProvider = new ProviderProfile();

            var currentProviderDto = new ProviderProfileDto
            {
                RegisteredName = "Old Provider Name"
            };

            _mapper
                .Map<ProviderProfileDraft>(draftDto)
                .Returns(draftEntity);

            _editRepository
                .SaveProviderDraft(draftEntity, "user@test.com")
                .Returns(response);

            _editRepository
                .GetProviderDetails(draftDto.ProviderProfileId, 2)
                .Returns(currentProvider);

            _mapper
                .Map<ProviderProfileDto>(currentProvider)
                .Returns(currentProviderDto);

            _emailSender
                .SendProviderEditRequestSubmittedToCab(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .Returns(true);

            _emailSender
                .SendProviderEditRequestSubmittedToOfdia(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .Returns(true);

            // Act
            var result = await _service.SaveProviderDraft(
                draftDto,
                "user@test.com",
                2);

            // Assert
            Assert.Same(response, result);

            await _emailSender
                .Received(1)
                .SendProviderEditRequestSubmittedToCab(
                    "user@test.com",
                    "Old Provider Name",
                    Arg.Any<string>(),
                    Arg.Any<string>());

            await _emailSender
                .Received(1)
                .SendProviderEditRequestSubmittedToOfdia(
                    "Old Provider Name",
                    Arg.Any<string>(),
                    Arg.Any<string>());
        }

        [Fact]
        public async Task SaveProviderDraft_EmailThrows_ReturnsSuccessfulRepositoryResponse()
        {
            // Arrange
            var draftDto = new ProviderProfileDraftDto
            {
                ProviderProfileId = 1,
                RegisteredName = "New Provider Name"
            };

            var draftEntity = new ProviderProfileDraft();

            var response = new GenericResponse
            {
                Success = true
            };

            var currentProvider = new ProviderProfile();

            var currentProviderDto = new ProviderProfileDto
            {
                RegisteredName = "Old Provider Name"
            };

            _mapper
                .Map<ProviderProfileDraft>(draftDto)
                .Returns(draftEntity);

            _editRepository
                .SaveProviderDraft(draftEntity, "user@test.com")
                .Returns(response);

            _editRepository
                .GetProviderDetails(draftDto.ProviderProfileId, 2)
                .Returns(currentProvider);

            _mapper
                .Map<ProviderProfileDto>(currentProvider)
                .Returns(currentProviderDto);

            _emailSender
                .SendProviderEditRequestSubmittedToCab(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>())
                .ThrowsAsync(new Exception("Email error"));

            // Act
            var result = await _service.SaveProviderDraft(
                draftDto,
                "user@test.com",
                2);

            // Assert
            Assert.Same(response, result);

            _logger.Received(1).Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<object>(value =>
                    value.ToString()!.Contains("Email error")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public void GetPrimaryContactUpdates_NoChanges_ReturnsEmptyDictionaries()
        {
            // Arrange
            var current = new ProviderProfileDto
            {
                PrimaryContactFullName = "Test Person",
                PrimaryContactEmail = "test@test.com",
                PrimaryContactJobTitle = "Manager",
                PrimaryContactTelephoneNumber = "123456"
            };

            var previous = new ProviderProfileDto
            {
                PrimaryContactFullName = "Test Person",
                PrimaryContactEmail = "test@test.com",
                PrimaryContactJobTitle = "Manager",
                PrimaryContactTelephoneNumber = "123456"
            };

            // Act
            var result = _service.GetPrimaryContactUpdates(current, previous);

            // Assert
            Assert.Empty(result.Item1);
            Assert.Empty(result.Item2);
        }

        [Fact]
        public void GetPrimaryContactUpdates_AllFieldsChanged_ReturnsAllChanges()
        {
            // Arrange
            var current = new ProviderProfileDto
            {
                PrimaryContactFullName = "New Name",
                PrimaryContactEmail = "new@test.com",
                PrimaryContactJobTitle = "New Job",
                PrimaryContactTelephoneNumber = "222"
            };

            var previous = new ProviderProfileDto
            {
                PrimaryContactFullName = "Old Name",
                PrimaryContactEmail = "old@test.com",
                PrimaryContactJobTitle = "Old Job",
                PrimaryContactTelephoneNumber = "111"
            };

            // Act
            var result = _service.GetPrimaryContactUpdates(current, previous);

            // Assert
            Assert.Equal(4, result.Item1.Count);
            Assert.Equal(4, result.Item2.Count);
        }

        [Fact]
        public void GetSecondaryContactUpdates_NoChanges_ReturnsEmptyDictionaries()
        {
            // Arrange
            var current = new ProviderProfileDto
            {
                SecondaryContactFullName = "Test Person",
                SecondaryContactEmail = "test@test.com",
                SecondaryContactJobTitle = "Manager",
                SecondaryContactTelephoneNumber = "123456"
            };

            var previous = new ProviderProfileDto
            {
                SecondaryContactFullName = "Test Person",
                SecondaryContactEmail = "test@test.com",
                SecondaryContactJobTitle = "Manager",
                SecondaryContactTelephoneNumber = "123456"
            };

            // Act
            var result = _service.GetSecondaryContactUpdates(current, previous);

            // Assert
            Assert.Empty(result.Item1);
            Assert.Empty(result.Item2);
        }

        [Fact]
        public void GetSecondaryContactUpdates_AllFieldsChanged_ReturnsAllChanges()
        {
            // Arrange
            var current = new ProviderProfileDto
            {
                SecondaryContactFullName = "New Name",
                SecondaryContactEmail = "new@test.com",
                SecondaryContactJobTitle = "New Job",
                SecondaryContactTelephoneNumber = "222"
            };

            var previous = new ProviderProfileDto
            {
                SecondaryContactFullName = "Old Name",
                SecondaryContactEmail = "old@test.com",
                SecondaryContactJobTitle = "Old Job",
                SecondaryContactTelephoneNumber = "111"
            };

            // Act
            var result = _service.GetSecondaryContactUpdates(current, previous);

            // Assert
            Assert.Equal(4, result.Item1.Count);
            Assert.Equal(4, result.Item2.Count);
        }

        [Fact]
        public async Task UpdatePrimaryContact_ValidProvider_ReturnsRepositoryResponse()
        {
            // Arrange
            var providerDto = new ProviderProfileDto();

            var response = new GenericResponse
            {
                Success = true
            };

            _editRepository
                .UpdatePrimaryContact(
                    Arg.Any<ProviderProfile>(),
                    "user@test.com")
                .Returns(response);

            // Act
            var result = await _service.UpdatePrimaryContact(
                providerDto,
                "user@test.com");

            // Assert
            Assert.Same(response, result);

            _mapper.Received(1).Map(
                providerDto,
                Arg.Any<ProviderProfile>());
        }

        [Fact]
        public async Task UpdateSecondaryContact_ValidProvider_ReturnsRepositoryResponse()
        {
            // Arrange
            var providerDto = new ProviderProfileDto();

            var response = new GenericResponse
            {
                Success = true
            };

            _editRepository
                .UpdateSecondaryContact(
                    Arg.Any<ProviderProfile>(),
                    "user@test.com")
                .Returns(response);

            // Act
            var result = await _service.UpdateSecondaryContact(
                providerDto,
                "user@test.com");

            // Assert
            Assert.Same(response, result);

            _mapper.Received(1).Map(
                providerDto,
                Arg.Any<ProviderProfile>());
        }

        [Fact]
        public async Task UpdateCompanyInfoAndPublicProviderInfo_ValidProvider_ReturnsRepositoryResponse()
        {
            // Arrange
            var providerDto = new ProviderProfileDto();

            var response = new GenericResponse
            {
                Success = true
            };

            _editRepository
                .UpdateCompanyInfoAndPublicProviderInfo(
                    Arg.Any<ProviderProfile>(),
                    "user@test.com")
                .Returns(response);

            // Act
            var result =
                await _service.UpdateCompanyInfoAndPublicProviderInfo(
                    providerDto,
                    "user@test.com");

            // Assert
            Assert.Same(response, result);

            _mapper.Received(1).Map(
                providerDto,
                Arg.Any<ProviderProfile>());
        }
    }
}