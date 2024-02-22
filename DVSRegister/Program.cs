using DVSRegister;
using DVSRegister.Data;


var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration, builder.Environment);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

Console.WriteLine(environment);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile($"appsettings.{environment}.json", optional: true);

startup.ConfigureSystemsManager(builder.Configuration, environment);
startup.ConfigureServices(builder.Services);
var app = builder.Build();


using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DVSRegisterDbContext>();
startup.ConfigureDatabaseHealthCheck(dbContext);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

