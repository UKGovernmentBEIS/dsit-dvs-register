using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Controllers;
using DVSRegister.Extensions;
using DVSRegister.Models;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.Models.CabTrustFramework;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace DVSRegister.UnitTests.Controllers;

public class CabServiceReApplicationControllerTests : ControllerTestBase<CabServiceReApplicationController>
{
    public CabServiceReApplicationControllerTests()
    {
        ConfigureFakes(() => new CabServiceReApplicationController(CabService, UserService, Logger));
    }

    [Fact]
    public async Task ServiceDraftDetails_ServiceWithoutManualUnderpinning_ReturnsViewAndStoresSummary()
    {
        var service = CreateServiceDto(10);
        CabService.GetServiceDetails(10, 1).Returns(service);

        var result = await Controller.ServiceDraftDetails(10);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(service, view.Model);
        Assert.Equal(10, Session.Get<ServiceSummaryViewModel>("ServiceSummary")!.ServiceId);
        await CabService.DidNotReceive().IsManualServiceLinkedToMultipleServices(Arg.Any<int>());
    }

    [Fact]
    public async Task ServiceDraftDetails_ServiceWithManualUnderpinning_QueriesLinkStatusAndStoresIt()
    {
        var service = CreateServiceDto(10);
        service.ManualUnderPinningServiceId = 25;
        CabService.GetServiceDetails(10, 1).Returns(service);
        CabService.IsManualServiceLinkedToMultipleServices(25).Returns(true);

        var result = await Controller.ServiceDraftDetails(10);

        Assert.IsType<ViewResult>(result);
        Assert.True(service.IsManualServiceLinkedToMultipleServices);
        Assert.True(Session.Get<ServiceSummaryViewModel>("ServiceSummary")!.IsManualServiceLinkedToMultipleServices);
        await CabService.Received(1).IsManualServiceLinkedToMultipleServices(25);
    }

    [Fact]
    public async Task ServiceDraftDetails_ServiceLookupFails_PropagatesException()
    {
        CabService.GetServiceDetails(10, 1).Returns<Task<ServiceDto>>(_ => throw new InvalidOperationException("lookup failed"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ServiceDraftDetails(10));

        Assert.Equal("lookup failed", exception.Message);
    }

    [Theory]
    [InlineData("service-name", "ServiceName", "CabService")]
    [InlineData("service-url", "ServiceURL", "CabService")]
    [InlineData("company-address", "CompanyAddress", "CabService")]
    [InlineData("terms-of-use", "TermsOfUseUpload", "TrustFramework0_4")]
    [InlineData("roles", "ProviderRoles", "CabService")]
    [InlineData("vouching", "VouchingGuidance", "TrustFramework0_4")]
    [InlineData("service-type", "SelectServiceType", "TrustFramework0_4")]
    [InlineData("underpinning-status", "StatusOfUnderpinningService", "TrustFramework0_4")]
    [InlineData("published-underpinning", "SelectUnderpinningService", "TrustFramework0_4")]
    [InlineData("manual-underpinning-name", "SelectUnderpinningService", "TrustFramework0_4")]
    [InlineData("manual-provider-name", "UnderPinningProviderName", "TrustFramework0_4")]
    [InlineData("manual-cab", "SelectCabOfUnderpinningService", "TrustFramework0_4")]
    [InlineData("manual-expiry", "UnderPinningServiceExpiryDate", "TrustFramework0_4")]
    [InlineData("gpg45-answer", "ServiceGPG45Input", "TrustFramework0_4")]
    [InlineData("gpg45-profiles", "ServiceGPG45", "TrustFramework0_4")]
    [InlineData("gpg44-answer", "ServiceGPG44Input", "TrustFramework0_4")]
    [InlineData("gpg44-levels", "ServiceGPG44", "TrustFramework0_4")]
    [InlineData("schemes-answer", "HasSupplementarySchemesInput", "CabService")]
    [InlineData("schemes-selection", "SupplementarySchemes", "CabService")]
    [InlineData("scheme-profile", "SchemeGPG45", "TrustFramework0_4")]
    [InlineData("scheme-gpg44-answer", "SchemeGPG44Input", "TrustFramework0_4")]
    [InlineData("scheme-gpg44-levels", "SchemeGPG44", "TrustFramework0_4")]
    [InlineData("certificate", "CertificateUploadPage", "CabService")]
    [InlineData("issue-date", "ConfirmityIssueDate", "CabService")]
    [InlineData("expiry-date", "ConfirmityExpiryDate", "CabService")]
    [InlineData("complete", "ServiceSummary", "CabService")]
    public void ResumeSubmission_IncompleteOrCompleteSummary_RedirectsToExpectedStep(string scenario, string expectedAction, string expectedController)
    {
        var summary = CreateCompleteSummary();
        ApplyResumeScenario(summary, scenario);
        Session.Set("ServiceSummary", summary);

        var result = Controller.ResumeSubmission();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(expectedAction, redirect.ActionName);
        Assert.Equal(expectedController, redirect.ControllerName);
        if (scenario.StartsWith("scheme-"))
        {
            Assert.Equal(7, redirect.RouteValues!["schemeId"]);
        }
    }

    [Fact]
    public void ResumeSubmission_NoStoredSummary_RedirectsToServiceName()
    {
        var result = Controller.ResumeSubmission();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ServiceName", redirect.ActionName);
        Assert.Equal("CabService", redirect.ControllerName);
    }

    [Fact]
    public async Task BeforeYouSubmitNewCertificate_ValidProviderAndExpiredCertificate_ClearsCertificateAndReturnsView()
    {
        UserService.GetUser(Arg.Any<string>()).Returns(new CabUserDto { Id = 8, CabId = 1 });
        CabService.CheckValidCabAndProviderProfile(30, 1).Returns(true);
        var service = CreateServiceDto(20);
        service.ConformityExpiryDate = DateTime.Today.AddDays(-1);
        CabService.GetServiceDetails(20, 1).Returns(service);

        var result = await Controller.BeforeYouSubmitNewCertificate(40, 30, 20, true);

        Assert.IsType<ViewResult>(result);
        var summary = Session.Get<ServiceSummaryViewModel>("ServiceSummary")!;
        Assert.True(summary.IsResubmission);
        Assert.True(summary.IsReupload);
        Assert.Equal(8, summary.CabUserId);
        Assert.Equal(40, summary.ServiceKey);
        Assert.Equal(30, summary.ProviderProfileId);
        Assert.Null(summary.FileName);
        Assert.Null(summary.FileLink);
        Assert.Null(summary.FileSizeInKb);
        Assert.Null(summary.ConformityIssueDate);
        Assert.Null(summary.ConformityExpiryDate);
        Assert.Equal(40, Controller.ViewBag.serviceKey);
    }

    [Fact]
    public async Task BeforeYouSubmitNewCertificate_CertificateExpiresToday_PreservesCertificate()
    {
        UserService.GetUser(Arg.Any<string>()).Returns(new CabUserDto { Id = 8, CabId = 1 });
        CabService.CheckValidCabAndProviderProfile(30, 1).Returns(true);
        var service = CreateServiceDto(20);
        service.ConformityExpiryDate = DateTime.Today;
        CabService.GetServiceDetails(20, 1).Returns(service);

        await Controller.BeforeYouSubmitNewCertificate(40, 30, 20, false);

        var summary = Session.Get<ServiceSummaryViewModel>("ServiceSummary")!;
        Assert.False(summary.IsReupload);
        Assert.Equal("certificate.pdf", summary.FileName);
        Assert.Equal(DateTime.Today, summary.ConformityExpiryDate);
    }

    [Fact]
    public async Task BeforeYouSubmitNewCertificate_ProviderDoesNotBelongToCab_ThrowsArgumentException()
    {
        UserService.GetUser(Arg.Any<string>()).Returns(new CabUserDto { CabId = 1 });
        CabService.CheckValidCabAndProviderProfile(30, 1).Returns(false);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.BeforeYouSubmitNewCertificate(40, 30, 20, false));

        Assert.Equal("Invalid providerProfileId.", exception.Message);
        await CabService.DidNotReceive().GetServiceDetails(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public async Task BeforeYouSubmitNewCertificate_UserLookupFails_PropagatesException()
    {
        UserService.GetUser(Arg.Any<string>()).Returns<Task<CabUserDto>>(_ => throw new InvalidOperationException("user failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Controller.BeforeYouSubmitNewCertificate(40, 30, 20, false));
    }

    [Fact]
    public async Task ContinueSubmission_Reupload_RedirectsToTrustFrameworkWithoutInspectingApplications()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40, ProviderProfileId = 30, IsReupload = true });
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto>());

        var result = await Controller.ContinueSubmission();

        AssertRedirect(result, "SelectVersionOfTrustFrameWork", "TrustFramework0_4", 30);
    }

    [Fact]
    public async Task ContinueSubmission_NewSubmissionWithoutExistingApplications_RedirectsToTrustFramework()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40, ProviderProfileId = 30, IsReupload = false });
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto> { CreateServiceDto(0) });

        var result = await Controller.ContinueSubmission();

        AssertRedirect(result, "SelectVersionOfTrustFrameWork", "TrustFramework0_4", 30);
    }

    [Theory]
    [InlineData(ServiceStatusEnum.Submitted)]
    [InlineData(ServiceStatusEnum.RemovedUnderReassign)]
    [InlineData(ServiceStatusEnum.AwaitingRemovalConfirmation)]
    public async Task ContinueSubmission_BlockingApplicationExists_RedirectsToRemovalStart(ServiceStatusEnum status)
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40, IsReupload = false });
        var service = CreateServiceDto(12);
        service.ServiceStatus = status;
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto> { service });

        var result = await Controller.ContinueSubmission();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("StartInProgressApplicationRemoval", redirect.ActionName);
    }

    [Fact]
    public async Task ContinueSubmission_InProgressUpdateRequested_RedirectsToRemovalStart()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40 });
        var service = CreateServiceDto(12);
        service.ServiceStatus = ServiceStatusEnum.UpdatesRequested;
        service.serviceDraft = new ServiceDraftDto { PreviousServiceStatus = ServiceStatusEnum.Submitted };
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto> { service });

        var result = await Controller.ContinueSubmission();

        Assert.Equal("StartInProgressApplicationRemoval", Assert.IsType<RedirectToActionResult>(result).ActionName);
    }

    [Fact]
    public async Task ContinueSubmission_ServiceListLookupFails_PropagatesException()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40 });
        CabService.GetServiceList(40, 1).Returns<Task<List<ServiceDto>>>(_ => throw new InvalidOperationException("list failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ContinueSubmission());
    }

    [Fact]
    public async Task StartInProgressApplicationRemoval_BlockingApplication_ReturnsItsDetailsInView()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40, ProviderProfileId = 30, ServiceId = 20 });
        var blockingService = CreateServiceDto(12);
        blockingService.ServiceStatus = ServiceStatusEnum.Submitted;
        var details = CreateServiceDto(12);
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto> { blockingService });
        CabService.GetServiceDetailsWithProvider(12, 1).Returns(details);

        var result = await Controller.StartInProgressApplicationRemoval();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(details, view.Model);
        Assert.Equal(40, Controller.ViewBag.ServiceKey);
        Assert.Equal(30, Controller.ViewBag.ProviderProfileId);
        Assert.Equal(20, Controller.ViewBag.ServiceId);
        await CabService.Received(1).GetServiceDetailsWithProvider(12, 1);
    }

    [Fact]
    public async Task StartInProgressApplicationRemoval_NoServices_ThrowsNullReferenceException()
    {
        Session.Set("ServiceSummary", new ServiceSummaryViewModel { ServiceKey = 40 });
        CabService.GetServiceList(40, 1).Returns(new List<ServiceDto>());

        await Assert.ThrowsAsync<NullReferenceException>(() => Controller.StartInProgressApplicationRemoval());
    }

    private static ServiceDto CreateServiceDto(int id)
    {
        return new ServiceDto
        {
            Id = id,
            ProviderProfileId = 30,
            ServiceKey = 40,
            ServiceName = "Service",
            WebSiteAddress = "https://example.com",
            CompanyAddress = "Address",
            ServiceRoleMapping = [],
            ServiceQualityLevelMapping = [],
            ServiceIdentityProfileMapping = [],
            ServiceSupSchemeMapping = [],
            CertificateReview = [],
            PublicInterestCheck = [],
            CabUser = new CabUserDto { CabId = 1 },
            TrustFrameworkVersion = new TrustFrameworkVersionDto { Version = Constants.TFVersion1_0 },
            FileName = "certificate.pdf",
            FileLink = "certificate-link",
            FileSizeInKb = 12,
            ConformityIssueDate = DateTime.Today.AddYears(-1),
            ConformityExpiryDate = DateTime.Today.AddYears(1)
        };
    }

    private static ServiceSummaryViewModel CreateCompleteSummary()
    {
        return new ServiceSummaryViewModel
        {
            ProviderProfileId = 30,
            ServiceName = "Service",
            ServiceURL = "https://example.com",
            CompanyAddress = "Address",
            TOUFileName = "terms.pdf",
            TFVersionViewModel = new TFVersionViewModel
            {
                SelectedTFVersion = new TrustFrameworkVersionDto { Version = Constants.TFVersion1_0 }
            },
            RoleViewModel = new RoleViewModel { SelectedRoles = [new RoleDto()] },
            HasVouchingGuidance = true,
            ServiceType = ServiceTypeEnum.Neither,
            SelectCabViewModel = new SelectCabViewModel { SelectedCabId = 1 },
            HasGPG45 = false,
            IdentityProfileViewModel = new IdentityProfileViewModel { SelectedIdentityProfiles = [new IdentityProfileDto()] },
            HasGPG44 = false,
            QualityLevelViewModel = new QualityLevelViewModel
            {
                SelectedQualityofAuthenticators = [new QualityLevelDto()],
                SelectedLevelOfProtections = [new QualityLevelDto()]
            },
            HasSupplementarySchemes = false,
            SupplementarySchemeViewModel = new SupplementarySchemeViewModel { SelectedSupplementarySchemes = [] },
            SchemeIdentityProfileMapping = [],
            SchemeQualityLevelMapping = [],
            FileName = "certificate.pdf",
            ConformityIssueDate = DateTime.Today.AddYears(-1),
            ConformityExpiryDate = DateTime.Today.AddYears(1)
        };
    }

    private static void ApplyResumeScenario(ServiceSummaryViewModel summary, string scenario)
    {
        switch (scenario)
        {
            case "service-name": summary.ServiceName = null; break;
            case "service-url": summary.ServiceURL = null; break;
            case "company-address": summary.CompanyAddress = null; break;
            case "terms-of-use": summary.TOUFileName = null; break;
            case "roles": summary.RoleViewModel!.SelectedRoles = []; break;
            case "vouching": summary.HasVouchingGuidance = null; break;
            case "service-type": summary.ServiceType = (ServiceTypeEnum)0; break;
            case "underpinning-status": MakeWhiteLabelled(summary); summary.IsUnderpinningServicePublished = null; break;
            case "published-underpinning": MakeWhiteLabelled(summary); summary.IsUnderpinningServicePublished = true; break;
            case "manual-underpinning-name": MakeManualWhiteLabelled(summary); summary.UnderPinningServiceName = null; break;
            case "manual-provider-name": MakeManualWhiteLabelled(summary); summary.UnderPinningProviderName = null; break;
            case "manual-cab": MakeManualWhiteLabelled(summary); summary.SelectCabViewModel!.SelectedCabId = null; break;
            case "manual-expiry": MakeManualWhiteLabelled(summary); summary.UnderPinningServiceExpiryDate = null; break;
            case "gpg45-answer": summary.HasGPG45 = null; break;
            case "gpg45-profiles": summary.HasGPG45 = true; summary.IdentityProfileViewModel!.SelectedIdentityProfiles = []; break;
            case "gpg44-answer": summary.HasGPG44 = null; break;
            case "gpg44-levels": summary.HasGPG44 = true; summary.QualityLevelViewModel!.SelectedLevelOfProtections = []; break;
            case "schemes-answer": summary.HasSupplementarySchemes = null; break;
            case "schemes-selection": summary.HasSupplementarySchemes = true; break;
            case "scheme-profile": AddScheme(summary, includeIdentity: false, includeQuality: true); break;
            case "scheme-gpg44-answer": AddScheme(summary, includeIdentity: true, includeQuality: false); break;
            case "scheme-gpg44-levels": AddScheme(summary, includeIdentity: true, includeQuality: true, emptyQuality: true); break;
            case "certificate": summary.FileName = null; break;
            case "issue-date": summary.ConformityIssueDate = null; break;
            case "expiry-date": summary.ConformityExpiryDate = null; break;
            case "complete": break;
            default: throw new ArgumentOutOfRangeException(nameof(scenario));
        }
    }

    private static void MakeWhiteLabelled(ServiceSummaryViewModel summary)
    {
        summary.ServiceType = ServiceTypeEnum.WhiteLabelled;
        summary.SelectedUnderPinningServiceId = null;
    }

    private static void MakeManualWhiteLabelled(ServiceSummaryViewModel summary)
    {
        MakeWhiteLabelled(summary);
        summary.IsUnderpinningServicePublished = false;
        summary.UnderPinningServiceName = "Underpinning service";
        summary.UnderPinningProviderName = "Provider";
        summary.UnderPinningServiceExpiryDate = DateTime.Today.AddYears(1);
    }

    private static void AddScheme(ServiceSummaryViewModel summary, bool includeIdentity, bool includeQuality, bool emptyQuality = false)
    {
        summary.HasSupplementarySchemes = true;
        summary.SupplementarySchemeViewModel!.SelectedSupplementarySchemes = [new SupplementarySchemeDto { Id = 7 }];
        summary.SchemeIdentityProfileMapping = includeIdentity
            ? [new SchemeIdentityProfileMappingViewModel { SchemeId = 7, IdentityProfile = new IdentityProfileViewModel() }]
            : [];
        summary.SchemeQualityLevelMapping = includeQuality
            ? [new SchemeQualityLevelMappingViewModel
            {
                SchemeId = 7,
                HasGPG44 = emptyQuality,
                QualityLevel = new QualityLevelViewModel
                {
                    SelectedLevelOfProtections = emptyQuality ? [] : [new QualityLevelDto()],
                    SelectedQualityofAuthenticators = emptyQuality ? [] : [new QualityLevelDto()]
                }
            }]
            : [];
    }

    private static void AssertRedirect(IActionResult result, string action, string controller, int providerProfileId)
    {
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(action, redirect.ActionName);
        Assert.Equal(controller, redirect.ControllerName);
        Assert.Equal(providerProfileId, redirect.RouteValues!["providerProfileId"]);
    }
}
