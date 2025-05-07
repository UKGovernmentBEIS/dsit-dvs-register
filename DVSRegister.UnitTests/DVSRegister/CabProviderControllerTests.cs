using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.Controllers;
using DVSRegister.Models.CAB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.RegularExpressions;

namespace DVSRegister.UnitTests.DVSRegister
{
    public class CabProviderControllerTests
    {
        private readonly ICabService cabService;
        private readonly IUserService userService;
        private readonly ILogger<CabProviderController> logger;
        private readonly CabProviderController cabProviderController;      

        public CabProviderControllerTests()
        {
            cabService = Substitute.For<ICabService>();
            userService = Substitute.For<IUserService>();
            logger = Substitute.For<ILogger<CabProviderController>>();     
            cabProviderController = new CabProviderController(cabService, userService, logger)
            {
                ControllerContext = Substitute.For<ControllerContext>()
            };            

        }

        #region Test registed name

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


        public void RegisteredNameAcceptedCharacters_Test(string input, bool expectedIsValid)
        {
            string pattern = @"^[A-Za-zÀ-ž &@£$€¥(){}\[\]<>!«»“”'‘’?""/*=#%+0-9.,:;\\/-]+$";
            var isValid = Regex.IsMatch(input, pattern);
            Assert.Equal(expectedIsValid, isValid);
        }

        [Fact]
        public async Task SaveRegisteredName_Test()
        {
            ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", false, false);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TradingName", redirectResult.ActionName);
            Assert.False(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
        }

        [Fact]
        public async Task SaveRegisteredNameFromSummaryPage_Test()
        {
            ProfileSummaryViewModel profileSummaryViewModel = MockRegisteredNameDetails("Test registered name", true,false);
            var result = await cabProviderController.SaveRegisteredName(profileSummaryViewModel);
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.False(cabProviderController.ModelState["RegisteredName"].Errors.Count > 0);
            Assert.Equal("ProfileSummary", redirectResult.ActionName);

        }

       

        [Theory]
        [InlineData("", "Enter the digital identity and attribute provider's registered name",  false)]
        [InlineData(TestDataConstants.LongRegisteredName, "The company's registered name must be less than 161 characters",false)]
        [InlineData("Test", Constants.RegisteredNameExistsError, true)]
        public async Task RegisteredNameInvalid_Test(string? registeredName, string errorMessage, bool registerdNameExists)
        {
           
            ProfileSummaryViewModel profileSummaryViewModel = new();
            profileSummaryViewModel.RegisteredName = registeredName;        
            cabService.CheckProviderRegisteredNameExists(registeredName).Returns(registerdNameExists);
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

     



    }
}
