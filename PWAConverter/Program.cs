using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PWAConverter.Data;
using PWAConverter.Helpers;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;
using PWAConverter.Services.Concrete;
using PWAConverter.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers().AddNewtonsoftJson(opt=>opt.SerializerSettings.ReferenceLoopHandling= Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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

services.AddSingleton<IPWAConverterMongoContext, PWAConverterMongoContext>();

// configure automapper with all automapper profiles from this assembly
services.AddAutoMapper(typeof(Program));

// configure strongly typed settings object


builder.Services.AddCors(options => options.AddPolicy(name: "AcceptAll",
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));

// configure DI for application services
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IIconService, IconService>();
services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        options.Path = "v1/openapi/pwa-converter.yaml";
        options.DocumentName = "pwa-converter";
    });

    app.UseSwaggerUi3(options =>
    {
        options.Path = "/openapi";
        options.DocumentPath = "/v1/openapi/pwa-converter.yaml";
    });
}
app.UseCors("AcceptAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


