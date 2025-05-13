using System.Text.Json;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Controllers;
using DVSRegister.Models.CAB;
using DVSRegister.UnitTests.Helpers;
using DVSRegister.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabProviderControllerTests
    {
        private readonly ICabService cabService;
        private readonly IUserService userService;
        private readonly ILogger<CabProviderController> logger;
        private readonly CabProviderController cabProviderController;
        private readonly DefaultHttpContext httpContext;
        private readonly FakeSession session;

        public CabProviderControllerTests()
        {
            cabService = Substitute.For<ICabService>();
            userService = Substitute.For<IUserService>();
            logger = Substitute.For<ILogger<CabProviderController>>();
            
            session = new FakeSession();
            httpContext = new DefaultHttpContext {
                Session = session
            };
            
            cabProviderController = new CabProviderController(cabService, userService, logger) {
                ControllerContext = new ControllerContext {
                    HttpContext = httpContext
                }
            };
        }

        #region Test registered name

        [Theory]
        [InlineData("Hello World!", true)]
        [InlineData("Test123", true)]
        [InlineData("@£$€¥(){}[]<>!", true)]
        [InlineData("12345", true)]
        [InlineData("Hello@World", true)]
        [InlineData("Hello World 123", true)]
        [InlineData("Café", true)]
        [InlineData("Hello-World", true)]
        [InlineData("This is a test!", true)]
        [InlineData("Invalid\\Char", true)]
        [InlineData("Invalid#Char^", false)]
        [InlineData("", false)]
        [InlineData("Special Characters: !@#$%^&*()", false)]


        public void RegisteredName_AcceptsValidCharacters_WhenInputIsValid_Test(string input, bool expectedIsValid)
        {
            string pattern = @"^[A-Za-zÀ-ž &@£$€¥(){}\[\]<>!«»“”'‘’?""/*=#%+0-9.,:;\\/-]+$";
            var isValid = Regex.IsMatch(input, pattern);
            Assert.Equal(expectedIsValid, isValid);
        }

        [Fact]
        public async Task SaveRegisteredName_RedirectsToTradingName_WhenValidModelAndFromSummaryPageFalse_Test()
        {
            ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", false, false);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TradingName", redirectResult.ActionName);
            Assert.False(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
        }

        [Fact]
        public async Task SaveRegisteredName_RedirectsToProfileSummary_WhenFromSummaryPageTrue_Test()
        {
            ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", true,false);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.False(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
            Assert.Equal("ProfileSummary", redirectResult.ActionName);
        }
        
        [Fact]
        public async Task SaveRegisteredName_ReturnsViewWithErrors_WhenModelStateInvalid_Test()
        {
            var profileSummaryViewModel = new ProfileSummaryViewModel { RegisteredName = "" };
            cabProviderController.ModelState.AddModelError("RegisteredName", "This field is required.");
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("RegisteredName", viewResult.ViewName);
            Assert.True(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
        }

        [Fact]
        public async Task SaveRegisteredName_RedirectsToNextStep_WhenNoErrors_Test()
        {
            var profileSummaryViewModel = MockRegisteredNameDetails("Test Registered Name", false, false);
            cabService.CheckProviderRegisteredNameExists(Arg.Any<string>()).Returns(false);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("TradingName", redirectResult.ActionName);
        }

        [Theory]
        [InlineData("", "Enter the digital identity and attribute provider's registered name",  false)]
        [InlineData(TestDataConstants.LongRegisteredName, "The company's registered name must be less than 161 characters",false)]
        [InlineData("Test", Constants.RegisteredNameExistsError, true)]
        public async Task SaveRegisteredName_ReturnsViewWithError_WhenRegisteredNameExists_Test(string? registeredName, string errorMessage, bool registeredNameExists)
        {
            ProfileSummaryViewModel profileSummaryViewModel = new();
            profileSummaryViewModel.RegisteredName = registeredName;        
            cabService.CheckProviderRegisteredNameExists(registeredName).Returns(registeredNameExists);
            cabProviderController.ModelState.AddModelError("RegisteredName", errorMessage);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);

            Assert.NotNull(result);
            
            var redirectResult = Assert.IsType<ViewResult>(result);
            
            Assert.Equal("RegisteredName", redirectResult.ViewName);
            Assert.True(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
            Assert.Equal(errorMessage, cabProviderController.ModelState["RegisteredName"].Errors[0].ErrorMessage);
        }

        #endregion

        private ProfileSummaryViewModel MockRegisteredNameDetails(string registeredName, bool fromSummaryPage, bool registeredNameExists)
        {
            ProfileSummaryViewModel profileSummaryViewModel = new();
            profileSummaryViewModel.RegisteredName = registeredName;
            profileSummaryViewModel.FromSummaryPage = fromSummaryPage;
            cabService.CheckProviderRegisteredNameExists("").Returns(registeredNameExists);
            cabProviderController.ModelState.AddModelError("RegisteredName", "");
            cabProviderController.ModelState["RegisteredName"].Errors.Clear();
            return profileSummaryViewModel;
        }
        
        [Fact]
        public async Task SaveRegisteredName_RetainsEmailAndStoresProfileSummaryInSession_Test()
        {
            session.SetString("Email", JsonSerializer.Serialize("test@example.com"));
            
            var vm = MockRegisteredNameDetails("Test LTD", fromSummaryPage: false, registeredNameExists: false);
            cabService
                .CheckProviderRegisteredNameExists("Test LTD")
                .Returns(false);

            await cabProviderController.SaveRegisteredName(vm);

            Assert.Equal(JsonSerializer.Serialize("test@example.com"), 
                httpContext.Session.GetString("Email"));
            var stored = httpContext.Session.Get<ProfileSummaryViewModel>("ProfileSummary");
            Assert.NotNull(stored);
            Assert.Equal("Test LTD", stored.RegisteredName);
        }

    }
}
