using DVSRegister.BusinessLogic.Models.PreRegistration;
using DVSRegister.BusinessLogic.Services.PreAssessment;
using DVSRegister.CommonUtility.Models;
using DVSRegister.Controllers;
using DVSRegister.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class PreRegistrationControllerTests
    {
        private Mock<ILogger<PreRegistrationController>> loggerMock = new Mock<ILogger<PreRegistrationController>>();
        private readonly Mock<IPreRegistrationService> preRegistrationServiceMock = new Mock<IPreRegistrationService>();
        private PreRegistrationController controller;

        public PreRegistrationControllerTests()
        {
            // Arrange : mock controller common for all test cases
            controller = new PreRegistrationController(loggerMock.Object, preRegistrationServiceMock.Object, null);
        }


        #region Start Page
        [Fact]
        public void StartPage_Returns_ViewResult()
        {
            var result = controller.StartPage();
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("StartPage", viewResult.ViewName);
        }
        #endregion

        #region Select Application Sponsor

        [Fact]
        public void Continue_Saves_Data_To_Session_And_Redirects()
        {
            Mock<ISession> session = SetMockSession();
            var result = controller.Continue() as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("SelectApplicationSponsor", result.ActionName);
            // Verify session is set with PreRegistrationSummary
            session.Verify(s => s.Set("PreRegistrationSummary", It.IsAny<byte[]>()), Times.Once);
        }



        [Fact]
        public void SaveApplicationSponsorSelection_Redirects_To_Contact_If_ApplicationSponsor_Selected()
        {

            var viewModel = new SummaryViewModel { IsApplicationSponsor = true };
            Mock<ISession> session = SetMockSession();
            controller.ControllerContext.ModelState.SetModelValue("IsApplicationSponsor", new ValueProviderResult());
            var result = controller.SaveApplicationSponsorSelection(viewModel) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Contact", result.ActionName);
            session.Verify(s => s.Set("PreRegistrationSummary", It.IsAny<byte[]>()), Times.Once);
        }



        [Fact]
        public void SaveApplicationSponsorSelection_Redirects_To_Sponsor_If_ApplicationSponsor_False()
        {
                 
            var viewModel = new SummaryViewModel { IsApplicationSponsor = false };
            Mock<ISession> session = SetMockSession();
            controller.ModelState.SetModelValue("IsApplicationSponsor", new ValueProviderResult());
            var result = controller.SaveApplicationSponsorSelection(viewModel) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Sponsor", result.ActionName);
            session.Verify(s => s.Set("PreRegistrationSummary", It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void SaveApplicationSponsor_If_ApplicationSponsor_Not_Selected_And_Error_Present()
        {

            var viewModel = new SummaryViewModel { IsApplicationSponsor = null };
            controller.ModelState.AddModelError("IsApplicationSponsor", "Select yes if you are the application sponsor.");
            var result = controller.SaveApplicationSponsorSelection(viewModel) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal("SelectApplicationSponsor", result.ViewName);
            Assert.Same(viewModel, result.Model); // Ensure the original view model is returned
            Assert.Contains("Select yes if you are the application sponsor.", result.ViewData.ModelState["IsApplicationSponsor"].Errors[0].ErrorMessage); // Verify error message is present
        }
        #endregion


        #region contact
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Sponsor_Sets_ViewBag_And_Returns_Correct_ViewModel(bool fromSummaryPage)
        {
            SetMockPreRegistrationSummaryData(true, true);
            var result = controller.Contact(fromSummaryPage);
            var viewResult = Assert.IsType<ViewResult>(result);
            var actualViewModel = viewResult.ViewData.Model as ContactViewModel;
            Assert.NotNull(viewResult.ViewData.Model);
            Assert.Equal(fromSummaryPage, controller.ViewBag.fromSummaryPage);
            Assert.IsType<ContactViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Contact Name", actualViewModel?.FullName);
            Assert.Equal("contact@gmail.com", actualViewModel?.Email);
            Assert.Equal("Contact Job Title", actualViewModel?.JobTitle);
            Assert.Equal("123456789", actualViewModel?.TelephoneNumber);
        }

        [Fact]
        public void SaveContact_ValidContact_ReturnsRedirectToAction()
        {

            var contactViewModel = new ContactViewModel()
            {
                FullName = "Contact Name",
                Email = "test@test.gmail.com",
                JobTitle = "Contact Job title",
                TelephoneNumber = "+44 808 157 0192"
            };

            var result = controller.SaveContact(contactViewModel);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void SaveContact_InvalidContact_ReturnsView()
        {
            ContactViewModel contactViewModel = new ContactViewModel();
            controller.ModelState.AddModelError("Error", "Invalid");
            var result = controller.SaveContact(contactViewModel);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        #endregion

        #region Sponsor & contact

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Contact_ViewBag_And_Returns_Correct_ViewModel(bool fromSummaryPage)
        {
            SetMockPreRegistrationSummaryData(false, true);
            var result = controller.Sponsor(fromSummaryPage);
            var viewResult = Assert.IsType<ViewResult>(result);
            var actualViewModel = viewResult.ViewData.Model as SponsorViewModel;
            Assert.NotNull(viewResult.ViewData.Model);
            Assert.Equal(fromSummaryPage, controller.ViewBag.fromSummaryPage);
            Assert.IsType<SponsorViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Sponsor Name", actualViewModel?.SponsorFullName);
            Assert.Equal("sponsor@dsit.com", actualViewModel?.SponsorEmail);
            Assert.Equal("Sponsor job Title", actualViewModel?.SponsorJobTitle);
            Assert.Equal("12345678", actualViewModel?.SponsorTelephoneNumber);
            Assert.Equal("Contact Name", actualViewModel?.ContactViewModel?.FullName);
            Assert.Equal("contact@gmail.com", actualViewModel?.ContactViewModel?.Email);
            Assert.Equal("Contact Job Title", actualViewModel?.ContactViewModel?.JobTitle);
            Assert.Equal("123456789", actualViewModel?.ContactViewModel?.TelephoneNumber);
        }



        [Fact]
        public void SaveSponsor_ValidSponsor_ReturnsRedirectToAction()
        {

            var sponsorViewModel = new SponsorViewModel()
            {
                SponsorFullName = "Valid Full Name",
                SponsorJobTitle = "Valid Job Title",
                SponsorEmail = "valid@email.com",
                SponsorTelephoneNumber = "+44 808 157 0192",
                FromSummaryPage = false,
                ContactViewModel = new ContactViewModel()
                {
                    FullName = "Contact Name",
                    Email = "test@test.gmail.com",
                    JobTitle = "Contact Job title",
                    TelephoneNumber = "+44 808 157 0192"
                }
            };

            var result = controller.SaveSponsor(sponsorViewModel);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void SaveSponsor_InvalidSponsor_ReturnsView()
        {
            SponsorViewModel sponsorViewModel = new SponsorViewModel();
            controller.ModelState.AddModelError("Error", "Invalid Sponsor");
            var result = controller.SaveSponsor(sponsorViewModel);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        #endregion


        #region Countries
        [Fact]
        public async Task Country_ReturnsViewWithValidViewModel()
        {
            var preRegistrationService = new Mock<IPreRegistrationService>();
            preRegistrationService.Setup(service => service.GetCountries())
                .ReturnsAsync(new List<CountryDto> { new CountryDto { Id = 1, CountryName = "UK" } });
            var result = await controller.Country(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var actualViewModel = viewResult.ViewData.Model as CountryViewModel;
            var model = Assert.IsAssignableFrom<CountryViewModel>(viewResult.ViewData.Model);
        }


        #endregion

        #region Company
        [Fact]
        public void Company_Returns_Correct_ViewModel()
        {
            SetMockPreRegistrationSummaryData(false, true);
            var result = controller.Company();
            var viewResult = Assert.IsType<ViewResult>(result);
            var actualViewModel = viewResult.ViewData.Model as CompanyViewModel;
            Assert.NotNull(viewResult.ViewData.Model);

            Assert.IsType<CompanyViewModel>(viewResult.ViewData.Model);
            Assert.Equal("test name", actualViewModel?.RegisteredCompanyName);
            Assert.Equal("test", actualViewModel?.TradingName);
            Assert.Equal("12345678", actualViewModel?.CompanyRegistrationNumber);
            Assert.Equal("test", actualViewModel?.ParentCompanyRegisteredName);
            Assert.Equal("USA", actualViewModel?.ParentCompanyLocation);

        }

        [Fact]
        public void SaveCompany_ValidCompany_ReturnsRedirectToAction()
        {

            CompanyViewModel companyViewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "test name",
                TradingName = "test",
                CompanyRegistrationNumber = "12345678",
                HasParentCompany = true,
                ParentCompanyRegisteredName = "test",
                ParentCompanyLocation = "USA"
            };

            var result = controller.SaveCompany(companyViewModel);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public void SaveCompany_InvalidCompany_ReturnsView()
        {
            CompanyViewModel companyViewModel = new CompanyViewModel();
            controller.ModelState.AddModelError("Error", "Invalid");
            var result = controller.SaveCompany(companyViewModel);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        #endregion



        #region Summary
        [Fact]
        public void Summary_Returns_Correct_ViewModel()
        {
            SetMockPreRegistrationSummaryData(false, true);
            var result = controller.Summary(true);
            var viewResult = Assert.IsType<ViewResult>(result);
            var actualViewModel = viewResult.ViewData.Model as SummaryViewModel;
            Assert.NotNull(viewResult.ViewData.Model);
            Assert.IsType<SummaryViewModel>(viewResult.ViewData.Model);


        }

        [Fact]
        public async Task SaveSummary_Valid_ReturnsRedirectToAction()
        {

            SummaryViewModel summaryViewModel = new SummaryViewModel { ConfirmAccuracy = true };
            SetMockPreRegistrationSummaryData(true, true);
            preRegistrationServiceMock.Setup(x => x.SavePreRegistration(It.IsAny<PreRegistrationDto>()))
             .ReturnsAsync(new GenericResponse { Success = true, EmailSent = true });

            controller.ControllerContext.ModelState.SetModelValue("ConfirmAccuracy", new ValueProviderResult());
            var result = await controller.SaveSummaryAndSubmit(summaryViewModel);            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ApplicationComplete", redirectResult.ActionName);


        }
        [Fact]
        public async Task SaveSummaryAndSubmit_Failure_ReturnsRedirectToError()
        {

            SummaryViewModel summaryViewModel = new SummaryViewModel { ConfirmAccuracy = true };
            SetMockPreRegistrationSummaryData(true, true);
            preRegistrationServiceMock.Setup(x => x.SavePreRegistration(It.IsAny<PreRegistrationDto>()))
                .ReturnsAsync(new GenericResponse { Success = false });
            controller.ControllerContext.ModelState.SetModelValue("ConfirmAccuracy", new ValueProviderResult());
            var result = await controller.SaveSummaryAndSubmit(summaryViewModel);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("HandleException", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }
        #endregion

        #region Private Methods


        private void SetMockPreRegistrationSummaryData(bool isApplicationSponsor, bool confirmAccuracy)
        {
            SummaryViewModel summaryViewModel = GetSummaryViewModel(isApplicationSponsor, confirmAccuracy);

            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            MockHttpSession mockSession = new MockHttpSession();
            mockSession["PreRegistrationSummary"] = JsonConvert.SerializeObject(summaryViewModel);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);          
            controller.ControllerContext = new ControllerContext() { HttpContext = mockHttpContext.Object };

        }

        private static SummaryViewModel GetSummaryViewModel(bool isApplicationSponsor, bool confirmAccuracy)
        {
            SummaryViewModel summaryViewModel = new SummaryViewModel();
            summaryViewModel.IsApplicationSponsor = isApplicationSponsor;
            summaryViewModel.ConfirmAccuracy = confirmAccuracy;
            summaryViewModel.CountryViewModel = new CountryViewModel { SelectedCountries = new List<CountryDto> { new CountryDto { Id =1, CountryName = "UK" } } };
            summaryViewModel.CompanyViewModel = new CompanyViewModel
            {
                RegisteredCompanyName = "test name",
                TradingName = "test",
                CompanyRegistrationNumber = "12345678",
                HasParentCompany = true,
                ParentCompanyRegisteredName = "test",
                ParentCompanyLocation = "USA"
            };

            if (isApplicationSponsor)
            {
                summaryViewModel.SponsorViewModel =
                    new SponsorViewModel
                    {
                        ContactViewModel = new ContactViewModel
                        {
                            Email = "contact@gmail.com",
                            FullName = "Contact Name",
                            JobTitle = "Contact Job Title",
                            TelephoneNumber = "123456789"
                        }
                    };
            }

            else
            {
                summaryViewModel.SponsorViewModel =  new SponsorViewModel
                {
                    SponsorFullName = "Sponsor Name",
                    SponsorEmail = "sponsor@dsit.com",
                    SponsorTelephoneNumber = "12345678",
                    SponsorJobTitle = "Sponsor job Title",
                    ContactViewModel = new ContactViewModel
                    {
                        Email = "contact@gmail.com",
                        FullName = "Contact Name",
                        JobTitle = "Contact Job Title",
                        TelephoneNumber = "123456789"
                    }
                };
            }

            return summaryViewModel;
        }

        private Mock<ISession> SetMockSession()
        {
            var httpContext = new DefaultHttpContext();
            var session = new Mock<ISession>();

            httpContext.Session = session.Object;
            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            return session;
        }


        #endregion

    }
}
