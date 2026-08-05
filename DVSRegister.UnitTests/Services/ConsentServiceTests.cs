using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.Register;
using DVSRegister.CommonUtility.Email;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DVSRegister.UnitTests.Services
{
    public class ConsentServiceTests
    {
        private readonly IConsentRepository _consentRepository;
        private readonly IRegisterService _registerService;
        private readonly IMapper _mapper;
        private readonly IConsentEmailSender _emailSender;
        private readonly ConsentService _service;

        public ConsentServiceTests()
        {
            _consentRepository = Substitute.For<IConsentRepository>();
            _registerService = Substitute.For<IRegisterService>();
            _mapper = Substitute.For<IMapper>();
            _emailSender = Substitute.For<IConsentEmailSender>();
            _service = new ConsentService(_consentRepository, _registerService, _mapper, _emailSender);
        }

        #region GetTokenStatus

        [Fact]
        public async Task GetTokenStatus_SingleService_ReturnsStoredTokenStatus()
        {
            var tokenDetails = new TokenDetails { ServiceIds = new List<int> { 12 } };
            var service = new Service
            {
                Id = 12,
                OpeningLoopTokenStatus = TokenStatusEnum.RequestCompleted
            };
            _consentRepository.GetService(12).Returns(service);

            var result = await _service.GetTokenStatus(tokenDetails);

            Assert.Equal(TokenStatusEnum.RequestCompleted, result);
            await _consentRepository.Received(1).GetService(12);
        }

        [Fact]
        public async Task GetTokenStatus_ExpiredToken_ReturnsStoredTokenStatus()
        {
            var tokenDetails = new TokenDetails
            {
                IsExpired = true,
                ServiceIds = new List<int> { 12 }
            };
            _consentRepository.GetService(12).Returns(new Service
            {
                Id = 12,
                OpeningLoopTokenStatus = TokenStatusEnum.Requested
            });

            var result = await _service.GetTokenStatus(tokenDetails);

            Assert.Equal(TokenStatusEnum.Requested, result);
        }

        [Fact]
        public async Task GetTokenStatus_NullServiceIds_ReturnsNotAvailableWithoutRepositoryLookup()
        {
            var result = await _service.GetTokenStatus(new TokenDetails { ServiceIds = null });

            Assert.Equal(TokenStatusEnum.NA, result);
            await _consentRepository.DidNotReceive().GetService(Arg.Any<int>());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task GetTokenStatus_ServiceCountIsNotOne_ReturnsNotAvailableWithoutRepositoryLookup(int count)
        {
            var tokenDetails = new TokenDetails
            {
                ServiceIds = Enumerable.Range(1, count).ToList()
            };

            var result = await _service.GetTokenStatus(tokenDetails);

            Assert.Equal(TokenStatusEnum.NA, result);
            await _consentRepository.DidNotReceive().GetService(Arg.Any<int>());
        }

        [Fact]
        public async Task GetTokenStatus_MissingServiceRecord_ReturnsNotAvailable()
        {
            _consentRepository.GetService(99).Returns(new Service());

            var result = await _service.GetTokenStatus(
                new TokenDetails { ServiceIds = new List<int> { 99 } });

            Assert.Equal(TokenStatusEnum.NA, result);
        }

        [Fact]
        public async Task GetTokenStatus_RepositoryThrows_PropagatesException()
        {
            _consentRepository.GetService(Arg.Any<int>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetTokenStatus(new TokenDetails { ServiceIds = new List<int> { 12 } }));
        }

        #endregion

        #region GetService

        [Fact]
        public async Task GetService_ServiceWithoutTrustmark_ReturnsMappedService()
        {
            var service = new Service { Id = 12 };
            var serviceDto = new ServiceDto { Id = 12 };
            _consentRepository.GetService(12).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(serviceDto);

            var result = await _service.GetService(12);

            Assert.Same(serviceDto, result);
            _registerService.DidNotReceive().GetSVGLogoEndPoint(Arg.Any<string>());
        }

        [Fact]
        public async Task GetService_ServiceWithTrustmark_TransformsSvgLogoLink()
        {
            var service = new Service { Id = 12 };
            var serviceDto = new ServiceDto
            {
                Id = 12,
                TrustmarkNumber = new TrustmarkNumberDto { SvgLogoLink = "logo.svg" }
            };
            _consentRepository.GetService(12).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(serviceDto);
            _registerService.GetSVGLogoEndPoint("logo.svg").Returns("/register/logo.svg");

            var result = await _service.GetService(12);

            Assert.Equal("/register/logo.svg", result.TrustmarkNumber.SvgLogoLink);
            _registerService.Received(1).GetSVGLogoEndPoint("logo.svg");
        }

        [Fact]
        public async Task GetService_RepositoryThrows_PropagatesException()
        {
            _consentRepository.GetService(Arg.Any<int>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetService(12));
        }

        [Fact]
        public async Task GetService_MapperThrows_PropagatesException()
        {
            var service = new Service { Id = 12 };
            _consentRepository.GetService(12).Returns(service);
            _mapper.Map<ServiceDto>(service)
                .Returns(_ => throw new InvalidOperationException("mapping failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetService(12));
        }

        [Fact]
        public async Task GetService_LogoTransformationThrows_PropagatesException()
        {
            var service = new Service { Id = 12 };
            var serviceDto = new ServiceDto
            {
                TrustmarkNumber = new TrustmarkNumberDto { SvgLogoLink = "logo.svg" }
            };
            _consentRepository.GetService(12).Returns(service);
            _mapper.Map<ServiceDto>(service).Returns(serviceDto);
            _registerService.GetSVGLogoEndPoint("logo.svg")
                .Returns(_ => throw new InvalidOperationException("logo failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetService(12));
        }

        #endregion

        #region RemoveProceedApplicationConsentToken

        [Fact]
        public async Task RemoveProceedApplicationConsentToken_ExistingToken_ReturnsTrue()
        {
            _consentRepository
                .RemoveProceedApplicationConsentToken("token", "token-id", "user@example.com")
                .Returns(true);

            var result = await _service.RemoveProceedApplicationConsentToken(
                "token", "token-id", "user@example.com");

            Assert.True(result);
        }

        [Fact]
        public async Task RemoveProceedApplicationConsentToken_InvalidToken_ReturnsFalse()
        {
            _consentRepository
                .RemoveProceedApplicationConsentToken("invalid-token", "token-id", "user@example.com")
                .Returns(false);

            var result = await _service.RemoveProceedApplicationConsentToken(
                "invalid-token", "token-id", "user@example.com");

            Assert.False(result);
        }

        [Fact]
        public async Task RemoveProceedApplicationConsentToken_SameTokenUsedTwice_ReturnsRepositoryResultsInOrder()
        {
            _consentRepository
                .RemoveProceedApplicationConsentToken("token", "token-id", "user@example.com")
                .Returns(Task.FromResult(true), Task.FromResult(false));

            var firstResult = await _service.RemoveProceedApplicationConsentToken(
                "token", "token-id", "user@example.com");
            var secondResult = await _service.RemoveProceedApplicationConsentToken(
                "token", "token-id", "user@example.com");

            Assert.True(firstResult);
            Assert.False(secondResult);
            await _consentRepository.Received(2)
                .RemoveProceedApplicationConsentToken("token", "token-id", "user@example.com");
        }

        [Fact]
        public async Task RemoveProceedApplicationConsentToken_RepositoryThrows_PropagatesException()
        {
            _consentRepository
                .RemoveProceedApplicationConsentToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RemoveProceedApplicationConsentToken("token", "token-id", "user@example.com"));
        }

        #endregion

        #region GetProviderAndCertificateDetailsByOpeningLoopToken

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_LinkedService_ReturnsMappedServiceDetails()
        {
            var linkedService = new Service { Id = 42 };
            var consentToken = new ProceedApplicationConsentToken
            {
                ServiceId = 42,
                Service = linkedService
            };
            var serviceDetails = new Service { Id = 42 };
            var serviceDto = new ServiceDto { Id = 42 };
            _consentRepository.GetProceedApplicationConsentToken("token", "token-id").Returns(consentToken);
            _consentRepository.GetServiceDetails(42).Returns(serviceDetails);
            _mapper.Map<ServiceDto>(serviceDetails).Returns(serviceDto);

            var result = await _service.GetProviderAndCertificateDetailsByOpeningLoopToken("token", "token-id");

            Assert.Same(serviceDto, result);
            await _consentRepository.Received(1).GetServiceDetails(42);
        }

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_MissingLinkedService_ReturnsNull()
        {
            var consentToken = new ProceedApplicationConsentToken
            {
                ServiceId = 42,
                Service = null!
            };
            _consentRepository.GetProceedApplicationConsentToken("token", "token-id").Returns(consentToken);

            var result = await _service.GetProviderAndCertificateDetailsByOpeningLoopToken("token", "token-id");

            Assert.Null(result);
            await _consentRepository.DidNotReceive().GetServiceDetails(Arg.Any<int>());
        }

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_MissingTokenRecord_ThrowsNullReferenceException()
        {
            _consentRepository.GetProceedApplicationConsentToken("missing-token", "token-id")
                .Returns((ProceedApplicationConsentToken)null!);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.GetProviderAndCertificateDetailsByOpeningLoopToken("missing-token", "token-id"));
        }

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_TokenLookupThrows_PropagatesException()
        {
            _consentRepository
                .GetProceedApplicationConsentToken(Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("token lookup failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetProviderAndCertificateDetailsByOpeningLoopToken("token", "token-id"));
        }

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_ServiceLookupThrows_PropagatesException()
        {
            var consentToken = new ProceedApplicationConsentToken
            {
                ServiceId = 42,
                Service = new Service { Id = 42 }
            };
            _consentRepository.GetProceedApplicationConsentToken("token", "token-id").Returns(consentToken);
            _consentRepository.GetServiceDetails(42)
                .ThrowsAsync(new InvalidOperationException("service lookup failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetProviderAndCertificateDetailsByOpeningLoopToken("token", "token-id"));
        }

        [Fact]
        public async Task GetProviderAndCertificateDetailsByOpeningLoopToken_MapperThrows_PropagatesException()
        {
            var consentToken = new ProceedApplicationConsentToken
            {
                ServiceId = 42,
                Service = new Service { Id = 42 }
            };
            var serviceDetails = new Service { Id = 42 };
            _consentRepository.GetProceedApplicationConsentToken("token", "token-id").Returns(consentToken);
            _consentRepository.GetServiceDetails(42).Returns(serviceDetails);
            _mapper.Map<ServiceDto>(serviceDetails)
                .Returns(_ => throw new InvalidOperationException("mapping failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetProviderAndCertificateDetailsByOpeningLoopToken("token", "token-id"));
        }

        #endregion

        #region UpdateServiceStatus

        [Fact]
        public async Task UpdateServiceStatus_UpdateFails_ReturnsResponseWithoutSendingEmails()
        {
            var response = new GenericResponse { Success = false };
            _consentRepository.UpdateServiceStatus(42, "provider@example.com", "accept").Returns(response);

            var result = await _service.UpdateServiceStatus(
                42, "provider@example.com", "Provider Ltd", "Identity Service", "accept");

            Assert.Same(response, result);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendAgreementToProceedApplicationToDSIT(default!, default!);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendConfirmationToProceedApplicationToDIP(default!, default!);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendDeclineToProceedApplicationToDSIT(default!, default!);
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendConfirmationOfDeclineToProceedApplicationToDIP(default!, default!);
        }

        [Fact]
        public async Task UpdateServiceStatus_AcceptedWithMultipleRecipients_SendsAcceptanceEmailsToEachRecipient()
        {
            var response = new GenericResponse { Success = true };
            _consentRepository
                .UpdateServiceStatus(42, "first@example.com;second@example.com", "accept")
                .Returns(response);
            _emailSender.SendAgreementToProceedApplicationToDSIT(Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);
            _emailSender.SendConfirmationToProceedApplicationToDIP(Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);

            var result = await _service.UpdateServiceStatus(
                42,
                "first@example.com;second@example.com",
                "Provider Ltd",
                "Identity Service",
                "accept");

            Assert.Same(response, result);
            await _emailSender.Received(1)
                .SendAgreementToProceedApplicationToDSIT("Provider Ltd", "Identity Service");
            await _emailSender.Received(1)
                .SendConfirmationToProceedApplicationToDIP("Identity Service", "first@example.com");
            await _emailSender.Received(1)
                .SendConfirmationToProceedApplicationToDIP("Identity Service", "second@example.com");
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendDeclineToProceedApplicationToDSIT(default!, default!);
        }

        [Fact]
        public async Task UpdateServiceStatus_Declined_SendsDeclineEmails()
        {
            var response = new GenericResponse { Success = true };
            _consentRepository.UpdateServiceStatus(42, "provider@example.com", "decline").Returns(response);
            _emailSender.SendDeclineToProceedApplicationToDSIT(Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);
            _emailSender.SendConfirmationOfDeclineToProceedApplicationToDIP(Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);

            var result = await _service.UpdateServiceStatus(
                42, "provider@example.com", "Provider Ltd", "Identity Service", "decline");

            Assert.Same(response, result);
            await _emailSender.Received(1)
                .SendDeclineToProceedApplicationToDSIT("Provider Ltd", "Identity Service");
            await _emailSender.Received(1)
                .SendConfirmationOfDeclineToProceedApplicationToDIP("Identity Service", "provider@example.com");
            await _emailSender.DidNotReceiveWithAnyArgs()
                .SendAgreementToProceedApplicationToDSIT(default!, default!);
        }

        [Fact]
        public async Task UpdateServiceStatus_RepositoryThrows_PropagatesException()
        {
            _consentRepository.UpdateServiceStatus(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateServiceStatus(
                42, "provider@example.com", "Provider Ltd", "Identity Service", "accept"));
        }

        [Fact]
        public async Task UpdateServiceStatus_EmailSenderThrows_PropagatesException()
        {
            _consentRepository.UpdateServiceStatus(42, "provider@example.com", "accept")
                .Returns(new GenericResponse { Success = true });
            _emailSender.SendAgreementToProceedApplicationToDSIT(Arg.Any<string>(), Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("email failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateServiceStatus(
                42, "provider@example.com", "Provider Ltd", "Identity Service", "accept"));
        }

        #endregion

        #region GetDownloadTokenFromTokenId

        [Fact]
        public async Task GetDownloadTokenFromTokenId_ExistingToken_ReturnsMappedToken()
        {
            var token = new DownloadLogoToken { TokenId = "token-id", ServiceId = 42 };
            var tokenDto = new DownloadLogoTokenDto { TokenId = "token-id", ServiceId = 42 };
            _consentRepository.GetDownloadLogoToken("token-id").Returns(token);
            _mapper.Map<DownloadLogoTokenDto>(token).Returns(tokenDto);

            var result = await _service.GetDownloadTokenFromTokenId("token-id");

            Assert.Same(tokenDto, result);
        }

        [Fact]
        public async Task GetDownloadTokenFromTokenId_MissingTokenRecord_ReturnsNull()
        {
            _consentRepository.GetDownloadLogoToken("missing-token-id")
                .Returns((DownloadLogoToken?)null);

            var result = await _service.GetDownloadTokenFromTokenId("missing-token-id");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDownloadTokenFromTokenId_RepositoryThrows_PropagatesException()
        {
            _consentRepository.GetDownloadLogoToken(Arg.Any<string>())
                .ThrowsAsync(new InvalidOperationException("repository failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetDownloadTokenFromTokenId("token-id"));
        }

        [Fact]
        public async Task GetDownloadTokenFromTokenId_MapperThrows_PropagatesException()
        {
            var token = new DownloadLogoToken { TokenId = "token-id", ServiceId = 42 };
            _consentRepository.GetDownloadLogoToken("token-id").Returns(token);
            _mapper.Map<DownloadLogoTokenDto>(token)
                .Returns(_ => throw new InvalidOperationException("mapping failure"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetDownloadTokenFromTokenId("token-id"));
        }

        #endregion
    }
}
