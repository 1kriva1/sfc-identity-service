using SFC.Identity.Api;
using SFC.Identity.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder
       .ConfigureServices()
       .ConfigurePipeline();

if (app.Environment.IsDevelopment())
{
    await app.ResetDatabaseAsync();
}

app.Run();

public partial class Program { }