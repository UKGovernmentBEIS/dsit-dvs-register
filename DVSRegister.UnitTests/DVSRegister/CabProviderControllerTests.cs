using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Controllers;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabProviderControllerTests
    {
        private readonly ICabService cabService;
        private readonly IUserService userService;
        private readonly ILogger<CabProviderController> logger;
        private readonly CabProviderController cabProviderController;     
        private ISession session;
        private Dictionary<string, byte[]> sessionStore;

        public CabProviderControllerTests()
        {
          
            session = Substitute.For<ISession>();
            cabService = Substitute.For<ICabService>();
            userService = Substitute.For<IUserService>();
            logger = Substitute.For<ILogger<CabProviderController>>();
           
            cabProviderController = new CabProviderController(cabService, userService, logger)
            {
                ControllerContext = Substitute.For<ControllerContext>()
            };
            InitializeHttpContextAndSession();
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
            //mock required session key value

            session.Set("Email", "test@test123");
            ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", false, false);
            session.Set("ProfileSummary", profileSummaryViewModel);
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

        private void InitializeHttpContextAndSession()
        {

            sessionStore = new Dictionary<string, byte[]>();
            session.When(s => s.Set(Arg.Any<string>(), Arg.Any<byte[]>()))
                  .Do(call =>
                  {
                      var key = call.Arg<string>();
                      var value = call.Arg<byte[]>();
                      sessionStore[key] = value;
                  });

            session.TryGetValue(Arg.Any<string>(), out Arg.Any<byte[]>())
                   .Returns(call =>
                   {
                       var key = call.Arg<string>();
                       var found = sessionStore.TryGetValue(key, out var value);
                       call[1] = value;
                       return found;
                   });
            var httpContext = new DefaultHttpContext();
            httpContext.Session = session;
            cabProviderController.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
    }
}
