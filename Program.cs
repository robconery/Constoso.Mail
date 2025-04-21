using Microsoft.OpenApi.Models;
using Contoso.Data;
using Contoso.Mail.Services;

//load up the config from env and appsettings
var config = Viper.Config("Integration");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IDb, DB>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
if (config.Get("SEND_WORKER") == "local")
{
  builder.Services.AddHostedService<BackgroundSend>();
}
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "0.0.1",
    Title = "Contoso Mail Services API",
    Description = "Transactional and bulk email sending services for Contoso.",
    Contact = new OpenApiContact
    {
      Name = "Rob Conery, Aaron Wislang, and the Contoso Team",
      Url = new Uri("https://contosotraders.dev")
    },
    License = new OpenApiLicense
    {
      Name = "MIT",
      Url = new Uri("https://opensource.org/license/mit/")
    }
  });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});

// Initialize DB connection but don't store the unused connection
DB.Sqlite();
Contoso.Mail.Api.PublicRoutes.MapRoutes(app);
Contoso.Mail.Api.Admin.BroadcastRoutes.MapRoutes(app);
Contoso.Mail.Api.Admin.ContactRoutes.MapRoutes(app);
Contoso.Mail.Api.Admin.BulkOperationRoutes.MapRoutes(app);

app.Run();

//this is for tests
public partial class Program { }