using DVSRegister.BusinessLogic.Models;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.CommonUtility;
using DVSRegister.CommonUtility.Models.Enums;
using DVSRegister.Data.Entities;
using DVSRegister.Data.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text;

namespace DVSRegister.UnitTests.Services
{
    public class ActionLogServiceTests
    {
        private readonly IActionLogRepository _actionLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ActionLogService> _logger;
        private readonly ActionLogService _service;

        public ActionLogServiceTests()
        {
            _actionLogRepository = Substitute.For<IActionLogRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _logger = Substitute.For<ILogger<ActionLogService>>();

            _service = new ActionLogService(_actionLogRepository, _userRepository, _logger);
        }


        [Fact]
        public async Task SaveActionLogs_ShouldSave_ForBusinessDetailsUpdate_WithAllFields()
        {
            // Arrange
            var previousData = new Dictionary<string, List<string>>
            {
                { Constants.RegisteredName, new List<string> { "Old Registered Name" } },
                { Constants.TradingName, new List<string> { "Old Trading Name" } },
                { Constants.CompanyRegistrationNumber, new List<string> { "Old Reg Number" } },
                { Constants.ParentCompanyRegisteredName, new List<string> { "Old Parent Name" } },
                { Constants.ParenyCompanyLocation, new List<string> { "Old Parent Location" } }
            };

            var updatedData = new Dictionary<string, List<string>>
            {
                { Constants.RegisteredName, new List<string> { "New Registered Name" } },
                { Constants.TradingName, new List<string> { "New Trading Name" } },
                { Constants.CompanyRegistrationNumber, new List<string> { "New Reg Number" } },
                { Constants.ParentCompanyRegisteredName, new List<string> { "New Parent Name" } },
                { Constants.ParenyCompanyLocation, new List<string> { "New Parent Location" } }
            };

            var dto = new ActionLogsDto
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = ActionDetailsEnum.BusinessDetailsUpdate,
                LoggedInUserEmail = "test@domain.com",
                ProviderId = 1,
                ProviderName = "Test Provider",
                PreviousData = previousData,
                UpdatedData = updatedData
            };

            var user = new CabUser { Id = 99 };
            var category = new ActionCategory { Id = 4 };
            var details = new ActionDetails { Id = 16 };

            _userRepository.GetUser(dto.LoggedInUserEmail).Returns(user);
            _actionLogRepository.GetActionCategory(dto.ActionCategoryEnum).Returns(category);
            _actionLogRepository.GetActionDetails(dto.ActionDetailsEnum).Returns(details);
          
            await _service.SaveActionLogs(dto);          
            await _actionLogRepository.Received(1).SaveActionLogs(Arg.Is<ActionLogs>(log =>
                log.ActionCategoryId == category.Id &&
                log.ActionDetailsId == details.Id &&
                log.CabUserId == user.Id &&
                log.ProviderProfileId == dto.ProviderId &&            
                log.DisplayMessage.Contains($"{Constants.RegisteredName} Old Registered Name to New Registered Name") &&
                log.DisplayMessage.Contains($"{Constants.TradingName} Old Trading Name to New Trading Name") &&
                log.DisplayMessage.Contains($"{Constants.CompanyRegistrationNumber} Old Reg Number to New Reg Number") &&
                log.DisplayMessage.Contains($"{Constants.ParentCompanyRegisteredName} Old Parent Name to New Parent Name") &&
                log.DisplayMessage.Contains($"{Constants.ParenyCompanyLocation} Old Parent Location to New Parent Location") &&            
                log.OldValues.RootElement.GetProperty(Constants.RegisteredName).EnumerateArray().First().GetString() == "Old Registered Name" &&
                log.NewValues.RootElement.GetProperty(Constants.RegisteredName).EnumerateArray().First().GetString() == "New Registered Name"
            ));
        }



        [Fact]
        public async Task SaveActionLogs_ShouldHideRegisterUpdates_ForProviderContactUpdateWithoutPublicContact()
        {
           
            var previousData = new Dictionary<string, List<string>>
                {
                { Constants.PrimaryContactName, new List<string> { "Old Contact Name" } }
                };

                var updatedData = new Dictionary<string, List<string>>
                {
                    { Constants.PrimaryContactName, new List<string> { "New Contact Name" } }
                };

            var dto = new ActionLogsDto
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = ActionDetailsEnum.ProviderContactUpdate,
                LoggedInUserEmail = "test@domain.com",
                ProviderId = 1,
                ProviderName = "Test Provider",
                PreviousData = previousData,
                UpdatedData = updatedData
            };

            var user = new CabUser { Id = 99 };
            var category = new ActionCategory { Id = 4 };
            var details = new ActionDetails { Id = 15 };

            _userRepository.GetUser(dto.LoggedInUserEmail).Returns(user);
            _actionLogRepository.GetActionCategory(dto.ActionCategoryEnum).Returns(category);
            _actionLogRepository.GetActionDetails(dto.ActionDetailsEnum).Returns(details);
          
            await _service.SaveActionLogs(dto);

            
            await _actionLogRepository.Received(1).SaveActionLogs(Arg.Is<ActionLogs>(log =>
                log.ShowInRegisterUpdates == false &&
                log.DisplayMessage == dto.ProviderName
            ));
        }

        [Fact]
        public async Task SaveActionLogs_ShouldShowRegisterUpdates_ForProviderContactUpdateWithPublicContactKeys()
        {
           
            var previousData = new Dictionary<string, List<string>>
            {
                { Constants.ProviderWebsiteAddress, new List<string> { "http://oldsite.com" } },
                { Constants.PublicContactEmail, new List<string> { "oldemail@test.com" } },
                { Constants.ProviderTelephoneNumber, new List<string> { "123456789" } }
            };

            var updatedData = new Dictionary<string, List<string>>
            {
                { Constants.ProviderWebsiteAddress, new List<string> { "http://newsite.com" } },
                { Constants.PublicContactEmail, new List<string> { "newemail@test.com" } },
                { Constants.ProviderTelephoneNumber, new List<string> { "987654321" } }
            };

            var dto = new ActionLogsDto
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = ActionDetailsEnum.ProviderContactUpdate,
                LoggedInUserEmail = "test@domain.com",
                ProviderId = 1,
                ProviderName = "Test Provider",
                PreviousData = previousData,
                UpdatedData = updatedData
            };

            var user = new CabUser { Id = 99 };
            var category = new ActionCategory { Id = 4 };
            var details = new ActionDetails { Id = 15 };

            _userRepository.GetUser(dto.LoggedInUserEmail).Returns(user);
            _actionLogRepository.GetActionCategory(dto.ActionCategoryEnum).Returns(category);
            _actionLogRepository.GetActionDetails(dto.ActionDetailsEnum).Returns(details);

            // Act
            await _service.SaveActionLogs(dto);

            // Assert
            await _actionLogRepository.Received(1).SaveActionLogs(Arg.Is<ActionLogs>(log =>
                log.ShowInRegisterUpdates == true && 
                log.DisplayMessage == dto.ProviderName &&
                log.OldValues.RootElement.GetProperty(Constants.ProviderWebsiteAddress).EnumerateArray().First().GetString() == "http://oldsite.com" &&
                log.NewValues.RootElement.GetProperty(Constants.ProviderWebsiteAddress).EnumerateArray().First().GetString() == "http://newsite.com"
            ));
        }
        [Fact]
        public async Task SaveActionLogs_ShouldLogError_WhenExceptionOccurs()
        {
           
            var previousData = new Dictionary<string, List<string>>
            {
                { Constants.ProviderWebsiteAddress, new List<string> { "http://oldsite.com" } },
                { Constants.PublicContactEmail, new List<string> { "oldemail@test.com" } },
                { Constants.ProviderTelephoneNumber, new List<string> { "123456789" } }
            };

            var updatedData = new Dictionary<string, List<string>>
            {
                { Constants.ProviderWebsiteAddress, new List<string> { "http://newsite.com" } },
                { Constants.PublicContactEmail, new List<string> { "newemail@test.com" } },
                { Constants.ProviderTelephoneNumber, new List<string> { "987654321" } }
            };

            var dto = new ActionLogsDto
            {
                ActionCategoryEnum = ActionCategoryEnum.ProviderUpdates,
                ActionDetailsEnum = ActionDetailsEnum.ProviderContactUpdate,
                LoggedInUserEmail = "test@domain.com",
                ProviderId = 0,
                PreviousData = previousData,
                UpdatedData = updatedData
            };

            var user = new CabUser { Id = 99 };
            var category = new ActionCategory { Id = 4 };
            var details = new ActionDetails { Id = 15 };

            _userRepository.GetUser(dto.LoggedInUserEmail).Returns(user);
            _actionLogRepository.GetActionCategory(dto.ActionCategoryEnum).Returns(category);
            _actionLogRepository.GetActionDetails(dto.ActionDetailsEnum).Returns(details);

            _actionLogRepository.SaveActionLogs(Arg.Any<ActionLogs>()).Throws(new Exception("DB error"));
        
            await _service.SaveActionLogs(dto);

            _logger.Received(1).Log(LogLevel.Error, Arg.Any<EventId>(),Arg.Is<object>(o => o.ToString().Contains("DB error")),Arg.Any<Exception>(),Arg.Any<Func<object, Exception, string>>() );
        }

    }
}
