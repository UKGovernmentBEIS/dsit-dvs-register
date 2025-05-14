using AutoMapper;
using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Controllers;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using DVSRegister.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models;
using Xunit;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabServiceAmendmentControllerTests
    {
        private readonly ICabService          cabService;
        private readonly IBucketService       bucketService;
        private readonly IMapper              mapper;
        private readonly ILogger<CabServiceAmendmentController> logger;
        private readonly CabServiceAmendmentController controller;
        private readonly DefaultHttpContext   httpContext;
        private readonly FakeSession          session;

        public CabServiceAmendmentControllerTests()
        {
            cabService   = Substitute.For<ICabService>();
            bucketService = Substitute.For<IBucketService>();
            mapper       = Substitute.For<IMapper>();
            logger       = Substitute.For<ILogger<CabServiceAmendmentController>>();

            session      = new FakeSession();
            httpContext  = new DefaultHttpContext { Session = session };

            controller = new CabServiceAmendmentController(cabService, logger, mapper, bucketService)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        #region ServiceAmendments GET

        [Fact]
        public async Task ServiceAmendments_SetsSessionAndReturnsView_Test()
        {
            const int serviceId = 42;
            var fakeDto = new ServiceDto
            {
                Id = serviceId,
                CertificateReview = new CertificateReviewDto {},
                CabUser = new CabUserDto { CabId = 123 }
            };
            cabService.GetServiceDetails(serviceId, Arg.Any<int>()).Returns(fakeDto);

            var result = await controller.ServiceAmendments(serviceId);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmendmentViewModel>(view.Model);

            Assert.Equal(fakeDto.CertificateReview, model.CertificateReview);

            var stored = httpContext.Session.Get<CertificateReviewDto>("CertificateReviewDetails");
            var expectedJson = JsonSerializer.Serialize(fakeDto.CertificateReview);
            var actualJson   = JsonSerializer.Serialize(stored);
            Assert.Equal(expectedJson, actualJson);
        }

        #endregion

        #region ServiceAmendmentsSummary GET

        [Fact]
        public void ServiceAmendmentsSummary_ReadsFromSessionAndReturnsView_Test()
        {
            var review = new CertificateReviewDto { };
            session.Set("CertificateReviewDetails", review);

            var result = controller.ServiceAmendmentsSummary();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmendmentViewModel>(view.Model);
            
            var expectedJson = JsonSerializer.Serialize(review);
            var actualJson   = JsonSerializer.Serialize(model.CertificateReview);
            Assert.Equal(expectedJson, actualJson);
        }

        #endregion

        #region SaveServiceAmendmentsSummary POST

        [Fact]
        public async Task SaveServiceAmendmentsSummary_SaveAction_CallsSaveAndRedirects_Test()
        {
            var summary = new ServiceSummaryViewModel {
                ServiceId   = 1,
                IsAmendment = true
            };
            session.Set("ServiceSummary", summary);

            var mappedDto = new ServiceDto { FileLink = "old.pdf", CabUser = new CabUserDto { CabId = 123 } };
            mapper
                .Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>())
                .Returns(mappedDto);

            cabService
                .GetServiceDetails(summary.ServiceId, Arg.Any<int>())
                .Returns(mappedDto);

            cabService
                .SaveServiceAmendments(
                    mappedDto,
                    mappedDto.FileLink,
                    mappedDto.CabUser.CabId,
                    Arg.Any<int>(),
                    Arg.Any<string>()
                )
                .Returns(new GenericResponse { Success = true });

            var result = await controller.SaveServiceAmendmentsSummary("save");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("InformationSubmitted", redirect.ActionName);
            Assert.Equal("CabService",         redirect.ControllerName);
        }

        [Fact]
        public async Task SaveServiceAmendmentsSummary_DiscardAction_RedirectsToServiceAmendments_Test()
        {
            var summary = new ServiceSummaryViewModel { ServiceId = 99, IsAmendment = false };
            session.Set("ServiceSummary", summary);
            
            var existing = new ServiceDto
            {
                FileLink = "old.pdf", 
                CabUser = new CabUserDto { CabId = 123 }
            };
            cabService
                .GetServiceDetails(summary.ServiceId, Arg.Any<int>())
                .Returns(existing);
            
            var mappedDto = new ServiceDto {
                FileLink = existing.FileLink,
                CabUser  = existing.CabUser
            };
            mapper
                .Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>())
                .Returns(mappedDto);
            
            cabService
                .CanDeleteCertificate(
                    mappedDto.FileLink,
                    existing.FileLink,
                    existing.CabUser.CabId,
                    Arg.Any<int>()
                )
                .Returns(true);
            
            var result = await controller.SaveServiceAmendmentsSummary("discard");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ServiceAmendments", redirect.ActionName);
            Assert.Null(redirect.ControllerName);
        }

        [Fact]
        public async Task SaveServiceAmendmentsSummary_InvalidAction_ThrowsArgumentException_Test()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                controller.SaveServiceAmendmentsSummary("not-a-valid-action")
            );
        }

        #endregion
    }
}
