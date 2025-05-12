using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Models.CAB;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.Controllers;
using DVSRegister.Models.CAB;
using DVSRegister.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using DVSRegister.CommonUtility.Models;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabRemovalRequestControllerTests
    {
        readonly ICabService               cabService;
        readonly ICabRemovalRequestService cabRemovalRequestService;
        readonly ILogger<CabRemovalRequestController> logger;
        readonly CabRemovalRequestController          controller;
        readonly HttpContext                          httpContext;
        readonly FakeSession                          session;

        public CabRemovalRequestControllerTests()
        {
            cabService               = Substitute.For<ICabService>();
            cabRemovalRequestService = Substitute.For<ICabRemovalRequestService>();
            logger                   = Substitute.For<ILogger<CabRemovalRequestController>>();

            var services = new ServiceCollection();

            var tempDataFactory = Substitute.For<ITempDataDictionaryFactory>();
            tempDataFactory
                .GetTempData(Arg.Any<HttpContext>())
                .Returns(_ => Substitute.For<ITempDataDictionary>());
            services.AddSingleton<ITempDataDictionaryFactory>(tempDataFactory);

            var urlFactory = Substitute.For<IUrlHelperFactory>();
            var urlHelper  = Substitute.For<IUrlHelper>();
            urlHelper
                .Action(Arg.Any<UrlActionContext>())
                .Returns("/dummy");
            urlFactory
                .GetUrlHelper(Arg.Any<ActionContext>())
                .Returns(urlHelper);
            services.AddSingleton<IUrlHelperFactory>(urlFactory);

            var provider = services.BuildServiceProvider();

            session     = new FakeSession();
            session.SetString("CabId", "123");
            httpContext = new DefaultHttpContext
            {
                Session         = session,
                RequestServices = provider
            };

            controller = new CabRemovalRequestController(
                cabService,
                cabRemovalRequestService,
                logger
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
        
        #region ReasonForRemoval GET

        [Fact]
        public void ReasonForRemoval_ReturnsViewWithCorrectModel_WhenSessionContainsReason_Test()
        {
            session.SetString("ReasonForRemoval", "Removal reason");
            var result = controller.ReasonForRemoval(1, 1, "service");
            
            var vr    = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RemovalRequestViewModel>(vr.Model);
            Assert.Equal("Removal reason", model.RemovalReasonByCab);
            Assert.Equal("service",        model.WhatToRemove);
        }

        [Fact]
        public void ReasonForRemoval_ReturnsViewWithEmptyModel_WhenSessionDoesNotContainReason_Test()
        {
            var result = controller.ReasonForRemoval(1, 1, "service");
            
            var vr    = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<RemovalRequestViewModel>(vr.Model);
            Assert.Empty(model.RemovalReasonByCab);
            Assert.Equal("service", model.WhatToRemove);
        }

        #endregion

        #region SaveReasonForRemoval POST

        [Fact]
        public void SaveReasonForRemoval_RedirectsToAboutToRemove_WhenModelStateIsValid_Test()
        {
            var vm = new RemovalRequestViewModel {
                RemovalReasonByCab = "Valid removal reason",
                ServiceId          = 1
            };
            controller.ModelState.Clear();
            session.SetString("ReasonForRemoval", vm.RemovalReasonByCab);

            var result = controller.SaveReasonForRemoval(vm);

            var rr = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AboutToRemove", rr.ActionName);
        }

        [Fact]
        public void SaveReasonForRemoval_ReturnsViewWithErrors_WhenModelStateIsInvalid_Test()
        {
            var vm = new RemovalRequestViewModel {
                RemovalReasonByCab = ""
            };
            controller.ModelState.AddModelError(
                "RemovalReasonByCab",
                "This field is required."
            );

            var result = controller.SaveReasonForRemoval(vm);

            var vr = Assert.IsType<ViewResult>(result);
            Assert.Equal("ReasonForRemoval", vr.ViewName);
            Assert.True(controller.ModelState["RemovalReasonByCab"].Errors.Count > 0);
        }

        #endregion

        #region AboutToRemove GET

        [Fact]
        public async Task AboutToRemove_ReturnsViewWithServiceDetails_WhenServiceIdIsValid_Test()
        {
            var dto = new ServiceDto { Id = 1 };
            cabService
                .GetServiceDetailsWithProvider(1, Arg.Any<int>())
                .Returns(dto);
            session.SetString("ReasonForRemoval", "Reason");
            session.SetString("WhatToRemove",      "Service");

            var result = await controller.AboutToRemove(1);

            var vr    = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ServiceDto>(vr.Model);
            Assert.Equal("Reason",       model.RemovalReasonByCab);
            Assert.Equal("Service",      vr.ViewData["WhatToRemove"]);
        }

        #endregion

        #region RequestRemoval POST

        [Fact]
        public async Task RequestRemoval_ThrowsArgumentException_WhenProviderIdIsInvalid_Test()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                controller.RequestRemoval(-1, 1)
            );
            Assert.Equal("RequestRemoval failed: Invalid ProviderId.", ex.Message);
        }

        [Fact]
        public async Task RequestRemoval_ThrowsArgumentException_WhenServiceIdIsInvalid_Test()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                controller.RequestRemoval(1, -1)
            );
            Assert.Equal("RequestRemoval failed: Invalid ServiceId.", ex.Message);
        }

        [Fact]
        public async Task RequestRemoval_ReturnsRemovalRequestedView_WhenRemovalIsSuccessful_Test()
        {
            session.SetString("ReasonForRemoval", "Removal reason");
            session.SetString("WhatToRemove",      "service");

            cabRemovalRequestService
              .UpdateRemovalStatus(
                 Arg.Any<int>(),
                 1, 1, Arg.Any<string>(),
                 "Removal reason", "service"
              )
              .Returns(new GenericResponse { Success = true });

            cabService
              .GetServiceDetailsWithProvider(1, Arg.Any<int>())
              .Returns(new ServiceDto { Id = 1 });

            var result = await controller.RequestRemoval(1, 1);

            var vr = Assert.IsType<ViewResult>(result);
            Assert.Equal("RemovalRequested", vr.ViewName);
        }

        #endregion
    }
}
