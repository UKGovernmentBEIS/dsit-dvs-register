using DVSRegister.Controllers;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabRemovalRequestControllerTests : ControllerTestBase<CabRemovalRequestController>
    {
        public CabRemovalRequestControllerTests()
        {
            ConfigureFakes(() =>
            new CabRemovalRequestController(CabService, CabRemovalRequestService, ActionLogService, Logger));
        }

        #region ReasonForRemoval GET

        //[Fact]
        //public void ReasonForRemoval_ReturnsViewWithCorrectModel_WhenSessionContainsReason_Test()
        //{
        //    Session.SetString("ReasonForRemoval", "Removal reason");
        //    var result = Controller.ReasonForRemoval(1, 1, "service");

        //    var vr    = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<RemovalRequestViewModel>(vr.Model);
        //    Assert.Equal("Removal reason", model.RemovalReasonByCab);
        //    Assert.Equal("service",        model.WhatToRemove);
        //}

        //[Fact]
        //public void ReasonForRemoval_ReturnsViewWithEmptyModel_WhenSessionDoesNotContainReason_Test()
        //{
        //    var result = Controller.ReasonForRemoval(1, 1, "service");

        //    var vr    = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<RemovalRequestViewModel>(vr.Model);
        //    Assert.Empty(model.RemovalReasonByCab);
        //    Assert.Equal("service", model.WhatToRemove);
        //}

        #endregion

        #region SaveReasonForRemoval POST

        [Fact]
        public void SaveReasonForRemoval_RedirectsToAboutToRemove_WhenModelStateIsValid_Test()
        {
            var vm = new RemovalRequestViewModel {
                RemovalReasonByCab = "Valid removal reason",
                ServiceId          = 1
            };
            Controller.ModelState.Clear();
            Session.SetString("ReasonForRemoval", vm.RemovalReasonByCab);

            var result = Controller.SaveReasonForRemoval(vm);

            var rr = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AboutToRemove", rr.ActionName);
        }

        [Fact]
        public void SaveReasonForRemoval_ReturnsViewWithErrors_WhenModelStateIsInvalid_Test()
        {
            var vm = new RemovalRequestViewModel {
                RemovalReasonByCab = ""
            };
            Controller.ModelState.AddModelError(
                "RemovalReasonByCab",
                "This field is required."
            );

            var result = Controller.SaveReasonForRemoval(vm);

            var vr = Assert.IsType<ViewResult>(result);
            Assert.Equal("ReasonForRemoval", vr.ViewName);
            Assert.True(Controller.ModelState["RemovalReasonByCab"].Errors.Count > 0);
        }

        #endregion

        #region AboutToRemove GET

        //[Fact]
        //public async Task AboutToRemove_ReturnsViewWithServiceDetails_WhenServiceIdIsValid_Test()
        //{
        //    var dto = new ServiceDto { Id = 1 };
        //    CabService
        //        .GetServiceDetailsWithProvider(1, Arg.Any<int>())
        //        .Returns(dto);
        //    Session.SetString("ReasonForRemoval", "Reason");
        //    Session.SetString("WhatToRemove",      "Service");

        //    var result = await Controller.AboutToRemove(1);

        //    var vr    = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<ServiceDto>(vr.Model);
        //    Assert.Equal("Reason",       model.RemovalReasonByCab);
        //    Assert.Equal("Service",      vr.ViewData["WhatToRemove"]);
        //}

        #endregion

        #region RequestRemoval POST

        [Fact]
        public async Task RequestRemoval_ThrowsArgumentException_WhenProviderIdIsInvalid_Test()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.RequestRemoval(-1, 1)
            );
            Assert.Equal("RequestRemoval failed: Invalid ProviderId.", ex.Message);
        }

        [Fact]
        public async Task RequestRemoval_ThrowsArgumentException_WhenServiceIdIsInvalid_Test()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.RequestRemoval(1, -1)
            );
            Assert.Equal("RequestRemoval failed: Invalid ServiceId.", ex.Message);
        }

        //[Fact]
        //public async Task RequestRemoval_ReturnsRemovalRequestedView_WhenRemovalIsSuccessful_Test()
        //{
        //    Session.SetString("ReasonForRemoval", "Removal reason");
        //    Session.SetString("WhatToRemove",      "service");

        //    CabRemovalRequestService
        //      .UpdateRemovalStatus(
        //         Arg.Any<int>(),
        //         1, 1, Arg.Any<string>(),
        //         "Removal reason", "service"
        //      )
        //      .Returns(new GenericResponse { Success = true });

        //    CabService
        //      .GetServiceDetailsWithProvider(1, Arg.Any<int>())
        //      .Returns(new ServiceDto { Id = 1 });

        //    var result = await Controller.RequestRemoval(1, 1);

        //    var vr = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("RemovalRequested", vr.ViewName);
        //}

        #endregion
    }
}
