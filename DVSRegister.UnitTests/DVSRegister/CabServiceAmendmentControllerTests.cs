using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.Controllers;
using DVSRegister.Models.CAB;
using DVSRegister.Models.CAB.Service;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Text.Json;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Extensions;
using DVSRegister.UnitTests.Helpers;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabServiceAmendmentControllerTests  : ControllerTestBase<CabServiceAmendmentController>
    {
        public CabServiceAmendmentControllerTests()
        {
            ConfigureFakes(() =>
                new CabServiceAmendmentController(CabService,Logger, Mapper, BucketService ));
        }

        #region ServiceAmendments GET

        [Fact]
        public async Task ServiceAmendments_SetsSessionAndReturnsView_Test()
        {
            const int serviceId = 42;
            var fakeDto = TestDataFactory.CreateServiceDto(serviceId);
            CabService
                .GetServiceDetails(serviceId, Arg.Any<int>())
                .Returns(fakeDto);
            
            var result = await Controller.ServiceAmendments(serviceId);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmendmentViewModel>(view.Model);

            Assert.Equal(fakeDto.CertificateReview, model.CertificateReview);

            var stored = HttpContext.Session.Get<CertificateReviewDto>("CertificateReviewDetails");
            var expectedJson = JsonSerializer.Serialize(fakeDto.CertificateReview);
            var actualJson = JsonSerializer.Serialize(stored);
            Assert.Equal(expectedJson, actualJson);
        }

        #endregion

        #region ServiceAmendmentsSummary GET

        [Fact]
        public void ServiceAmendmentsSummary_ReadsFromSessionAndReturnsView_Test()
        {
            var review = TestDataFactory.CreateCertificateReviewDto();
            Session.Set("CertificateReviewDetails", review);

            var result = Controller.ServiceAmendmentsSummary();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmendmentViewModel>(view.Model);

            var expectedJson = JsonSerializer.Serialize(review);
            var actualJson = JsonSerializer.Serialize(model.CertificateReview);
            Assert.Equal(expectedJson, actualJson);
        }

        #endregion

        #region SaveServiceAmendmentsSummary POST

        [Fact]
        public async Task SaveServiceAmendmentsSummary_SaveAction_CallsSaveAndRedirects_Test()
        {
            var summary = TestDataFactory.CreateServiceSummaryViewModel(serviceId: 1, isAmendment: true);
            Session.Set("ServiceSummary", summary);

            var mappedDto = TestDataFactory.CreateServiceDto(
                serviceId: 1,
                cabId: 123,
                review: TestDataFactory.CreateCertificateReviewDto()
            );
            
            mappedDto.FileLink = "old.pdf";

            Mapper
                .Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>())
                .Returns(mappedDto);

            CabService
                .GetServiceDetails(summary.ServiceId, Arg.Any<int>())
                .Returns(mappedDto);

            CabService
                .SaveServiceAmendments(
                    mappedDto,
                    mappedDto.FileLink,
                    mappedDto.CabUser.CabId,
                    Arg.Any<int>(),
                    Arg.Any<string>()
                )
                .Returns(new GenericResponse { Success = true });

            var result = await Controller.SaveServiceAmendmentsSummary("save");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("InformationSubmitted", redirect.ActionName);
            Assert.Equal("CabService", redirect.ControllerName);
        }

        [Fact]
        public async Task SaveServiceAmendmentsSummary_DiscardAction_RedirectsToServiceAmendments_Test()
        {
            var summary = TestDataFactory.CreateServiceSummaryViewModel(serviceId: 99, isAmendment: false);
            Session.Set("ServiceSummary", summary);

            var existing = TestDataFactory.CreateServiceDto(
                serviceId: summary.ServiceId,
                cabId: 123,
                review: TestDataFactory.CreateCertificateReviewDto()
            );
            CabService
                .GetServiceDetails(summary.ServiceId, Arg.Any<int>())
                .Returns(existing);

            var mappedDto = new ServiceDto
            {
                FileLink = existing.FileLink,
                CabUser = existing.CabUser
            };
            Mapper
                .Map<ServiceDto>(Arg.Any<ServiceSummaryViewModel>())
                .Returns(mappedDto);

            CabService
                .CanDeleteCertificate(
                    mappedDto.FileLink,
                    existing.FileLink,
                    existing.CabUser.CabId,
                    Arg.Any<int>()
                )
                .Returns(true);

            var result = await Controller.SaveServiceAmendmentsSummary("discard");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ServiceAmendments", redirect.ActionName);
            Assert.Null(redirect.ControllerName);
        }

        [Fact]
        public async Task SaveServiceAmendmentsSummary_InvalidAction_ThrowsArgumentException_Test()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.SaveServiceAmendmentsSummary("not-a-valid-action")
            );
        }

        #endregion
    }
}
