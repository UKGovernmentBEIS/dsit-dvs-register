using DVSRegister.CommonUtility;
using DVSRegister.Controllers;
using DVSRegister.Extensions;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabProviderControllerTests : ControllerTestBase<CabProviderController>
    {
        public CabProviderControllerTests()
        {
            ConfigureFakes(() =>
            {
                var controllerInstance = new CabProviderController(CabService, EditService, UserService,ActionLogService,Mapper, Logger);
                return controllerInstance;
            });
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

        //[Fact]
        //public async Task SaveRegisteredName_RedirectsToTradingName_WhenValidModelAndFromSummaryPageFalse_Test()
        //{
        //    ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", false, false);
        //    var result = await Controller.SaveRegisteredName(profileSummaryViewModel);
        //    Assert.NotNull(result);
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("TradingName", redirectResult.ActionName);
        //    Assert.False(Controller.ModelState["RegisteredName"].Errors.Count > 0);
        //}

        //[Fact]
        //public async Task SaveRegisteredName_RedirectsToProfileSummary_WhenFromSummaryPageTrue_Test()
        //{
        //    ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", true,false);
        //    var result = await Controller.SaveRegisteredName(profileSummaryViewModel);
        //    Assert.NotNull(result);
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.False(Controller.ModelState["RegisteredName"].Errors.Count > 0);
        //    Assert.Equal("ProfileSummary", redirectResult.ActionName);
        //}
        
        //[Fact]
        //public async Task SaveRegisteredName_ReturnsViewWithErrors_WhenModelStateInvalid_Test()
        //{
        //    var profileSummaryViewModel = new ProfileSummaryViewModel { RegisteredName = "" };
        //    Controller.ModelState.AddModelError("RegisteredName", "This field is required.");
        //    var result = await Controller.SaveRegisteredName(profileSummaryViewModel);
        //    var viewResult = Assert.IsType<ViewResult>(result);
            
        //    Assert.Equal("RegisteredName", viewResult.ViewName);
        //    Assert.True(Controller.ModelState["RegisteredName"].Errors.Count > 0);
        //}

        //[Fact]
        //public async Task SaveRegisteredName_RedirectsToNextStep_WhenNoErrors_Test()
        //{
        //    var profileSummaryViewModel = MockRegisteredNameDetails("Test Registered Name", false, false);
        //    CabService.CheckProviderRegisteredNameExists(Arg.Any<string>()).Returns(false);
        //    var result = await Controller.SaveRegisteredName(profileSummaryViewModel);
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            
        //    Assert.Equal("TradingName", redirectResult.ActionName);
        //}

        //[Theory]
        //[InlineData("", "Enter the digital identity and attribute provider's registered name",  false)]
        //[InlineData(TestDataConstants.LongRegisteredName, "The company's registered name must be less than 161 characters",false)]
        //[InlineData("Test", Constants.RegisteredNameExistsError, true)]
        //public async Task SaveRegisteredName_ReturnsViewWithError_WhenRegisteredNameExists_Test(string? registeredName, string errorMessage, bool registeredNameExists)
        //{
        //    ProfileSummaryViewModel profileSummaryViewModel = new();
        //    profileSummaryViewModel.RegisteredName = registeredName;        
        //    CabService.CheckProviderRegisteredNameExists(registeredName).Returns(registeredNameExists);
        //    Controller.ModelState.AddModelError("RegisteredName", errorMessage);
        //    var result = await Controller.SaveRegisteredName(profileSummaryViewModel);

        //    Assert.NotNull(result);
            
        //    var redirectResult = Assert.IsType<ViewResult>(result);
            
        //    Assert.Equal("RegisteredName", redirectResult.ViewName);
        //    Assert.True(Controller.ModelState["RegisteredName"].Errors.Count > 0);
        //    Assert.Equal(errorMessage, Controller.ModelState["RegisteredName"].Errors[0].ErrorMessage);
        //}

        #endregion

        private ProfileSummaryViewModel MockRegisteredNameDetails(string registeredName, bool fromSummaryPage, bool registeredNameExists)
        {
            ProfileSummaryViewModel profileSummaryViewModel = new();
            profileSummaryViewModel.RegisteredName = registeredName;
            profileSummaryViewModel.FromSummaryPage = fromSummaryPage;
            CabService.CheckProviderRegisteredNameExists("").Returns(registeredNameExists);
            Controller.ModelState.AddModelError("RegisteredName", "");
            Controller.ModelState["RegisteredName"].Errors.Clear();
            return profileSummaryViewModel;
        }
        
        //[Fact]
        //public async Task SaveRegisteredName_RetainsEmailAndStoresProfileSummaryInSession_Test()
        //{
        //    Session.SetString("Email", JsonSerializer.Serialize("test@example.com"));
            
        //    var vm = MockRegisteredNameDetails("Test LTD", fromSummaryPage: false, registeredNameExists: false);
        //    CabService
        //        .CheckProviderRegisteredNameExists("Test LTD")
        //        .Returns(false);

        //    await Controller.SaveRegisteredName(vm);

        //    Assert.Equal(JsonSerializer.Serialize("test@example.com"), 
        //        HttpContext.Session.GetString("Email"));
        //    var stored = HttpContext.Session.Get<ProfileSummaryViewModel>("ProfileSummary");
        //    Assert.NotNull(stored);
        //    Assert.Equal("Test LTD", stored.RegisteredName);
        //}

    }
}
