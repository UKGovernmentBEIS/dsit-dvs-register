using DVSRegister.CommonUtility;

namespace DVSRegister.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleware> logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestPath = context.Request.Path.ToString().ToLower();
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected occurred in request {RequestPath}", requestPath);

                if (ex.InnerException != null)
                {
                    logger.LogError(ex.InnerException, "Inner exception in ExceptionHandlerMiddleware.");
                }
                // Redirect to error page 
                string redirectPath = requestPath.Contains("cab-service")
                    ? Constants.CabRegistrationErrorPath
                    : Constants.CommonErrorPath;
                
                context.Response.Redirect(redirectPath);
            }
        }
    }
}
