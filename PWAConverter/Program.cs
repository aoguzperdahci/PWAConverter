using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PWAConverter;
using PWAConverter.Authorization;
using PWAConverter.Data;
using PWAConverter.Helpers;
using PWAConverter.Services;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddOpenApiDocument(options =>
{
    options.Title = "PWA Converter";
    options.DocumentName = "pwa-converter";
    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("auth"));
    options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT Token"));
    options.DocumentProcessors.Add(new SecurityDefinitionAppender("auth", new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        In = OpenApiSecurityApiKeyLocation.Header,
        Scheme = "bearer",
        BearerFormat = "jwt"
    }));
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Secret:Key").Value)),
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false
        };
    });

services.AddDbContext<PWAConverterContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// configure automapper with all automapper profiles from this assembly
services.AddAutoMapper(typeof(Program));

// configure strongly typed settings object


builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }));

// configure DI for application services
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IUserService, UserService>();
services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        //options.PostProcess = (document, request) =>
        //{
        //    document.Servers.Clear();
        //    document.Servers.Add(new OpenApiServer { Url = serverUrl });
        //};
        options.Path = "v1/openapi/pwa-converter.yaml";
        options.DocumentName = "pwa-converter";
    });

    app.UseSwaggerUi3(options =>
    {
        options.Path = "/openapi";
        options.DocumentPath = "/v1/openapi/pwa-converter.yaml";
    });
}
app.UseCors("NgOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


