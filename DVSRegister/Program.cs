using DVSRegister;
using DVSRegister.Data;
using DVSRegister.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSentry(); // Initialize Sentry
var startup = new Startup(builder.Configuration, builder.Environment);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

Console.WriteLine(environment);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile($"appsettings.{environment}.json", optional: true)
.AddEnvironmentVariables();


startup.ConfigureServices(builder.Services);
var app = builder.Build();


using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DVSRegisterDbContext>();
startup.ConfigureDatabaseHealthCheck(dbContext);

// Configure Forwarded Headers Middleware
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Clear the default settings for KnownNetworks and KnownProxies
forwardedHeadersOptions.KnownNetworks.Clear(); // Clear default networks
forwardedHeadersOptions.KnownProxies.Clear();  // Clear default proxies
app.UseForwardedHeaders(forwardedHeadersOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{   
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseMiddleware<BasicAuthMiddleware>();
}
else
{
    app.UseMiddleware<ExceptionHandlerMiddleware>();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllers();
app.Run();



