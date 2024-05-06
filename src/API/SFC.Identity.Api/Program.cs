using SFC.Identity.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder
       .ConfigureServices()
       .ConfigurePipeline();

app.Run();

public partial class Program { }