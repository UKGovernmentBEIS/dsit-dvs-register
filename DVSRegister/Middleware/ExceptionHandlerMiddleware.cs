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
                logger.LogError($"An unexpected error occurred: {ex}");               
                logger.LogError($"Stacktrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.Write("Inner Exception");
                    Console.Write(String.Concat(ex.InnerException.StackTrace, ex.InnerException.Message));
                }
                // Redirect to error page 
              
                if (requestPath.Contains("cab-service"))
                    context.Response.Redirect(Constants.CabRegistrationErrorPath);
                else
                    context.Response.Redirect(Constants.CommonErrorPath);
            }
        }
    }
}
