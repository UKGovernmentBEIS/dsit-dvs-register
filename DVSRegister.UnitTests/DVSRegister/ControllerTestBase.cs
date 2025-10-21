using System.Text.Json;
using AutoMapper;
using DVSRegister.BusinessLogic.Services;
using DVSRegister.BusinessLogic.Services.CAB;
using DVSRegister.CommonUtility;
using DVSRegister.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public abstract class ControllerTestBase<TController>
    where TController : Controller
{
    protected ICabService CabService { get; private set; }
    protected ICabRemovalRequestService CabRemovalRequestService { get; private set; }
    protected IUserService UserService  { get; private set; }

    protected IBucketService BucketService { get; private set; }
    protected IActionLogService ActionLogService { get; private set; }
    protected IMapper Mapper { get; private set; }
    protected ILogger<TController> Logger { get; private set; }

    protected DefaultHttpContext HttpContext { get; private set; }
    protected FakeSession Session { get; private set; }

    protected TController Controller { get; private set; }

    protected void ConfigureFakes(Func<TController> controllerFactory)
    {
        CabService    = Substitute.For<ICabService>();
        CabRemovalRequestService  = Substitute.For<ICabRemovalRequestService>();
        UserService  = Substitute.For<IUserService>();
        BucketService = Substitute.For<IBucketService>();
        Mapper        = Substitute.For<IMapper>();
        Logger        = Substitute.For<ILogger<TController>>();

        Session      = new FakeSession();
        
        Session.SetString("CabId", JsonSerializer.Serialize(123)); 
        HttpContext  = new DefaultHttpContext { Session = Session };
        
        var services = new ServiceCollection();

        var tempDataFactory = Substitute.For<ITempDataDictionaryFactory>();
        tempDataFactory
            .GetTempData(Arg.Any<HttpContext>())
            .Returns(_ => Substitute.For<ITempDataDictionary>());
        services.AddSingleton<ITempDataDictionaryFactory>(tempDataFactory);

        var urlFactory = Substitute.For<IUrlHelperFactory>();
        var urlHelper  = Substitute.For<IUrlHelper>();

        urlFactory.GetUrlHelper(Arg.Any<ActionContext>())
            .Returns(urlHelper);
        services.AddSingleton<IUrlHelperFactory>(urlFactory);

        HttpContext.RequestServices = services.BuildServiceProvider();

        Controller = controllerFactory();
        Controller.ControllerContext = new ControllerContext { HttpContext = HttpContext };
        Controller.Url = urlHelper;

    }
}