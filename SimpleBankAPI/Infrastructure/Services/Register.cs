using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleBankAPI.API.Factories;
using SimpleBankAPI.API.Formatters;
using SimpleBankAPI.API.Formatters.Interfaces;
using SimpleBankAPI.Application.Interfaces;
using SimpleBankAPI.Application.Services;
using SimpleBankAPI.Application.Services.Interfaces;
using SimpleBankAPI.Application.Validators.Interfaces;
using SimpleBankAPI.Domain.Interfaces;
using SimpleBankAPI.Infrastructure.Clients;
using SimpleBankAPI.Infrastructure.Data;
using SimpleBankAPI.Infrastructure.Repositories;

namespace SimpleBankAPI.Infrastructure.Services;

public static class Register
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true;
        }).AddMvcOptions(options =>
        {
            options.OutputFormatters.Clear();
            options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerOptions.Default)));
            options.OutputFormatters.Add(new StringOutputFormatter());
            options.OutputFormatters.Add(new CsvOutputFormatter());
        });
        services.AddDbContext<AccountContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(
                configuration.GetConnectionString("AccountContext")));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton<ICurrencyRate, CurrencyClient>();
        services.AddHttpClient<ICurrencyRate, CurrencyClient>(client => 
        client.BaseAddress = new Uri($"https://api.freecurrencyapi.com/v1/latest?apikey={configuration.GetValue<string>("CURRENCY_API_KEY")}"));
        services.AddSingleton<IFactory<IValidator?>, ValidatorFactory>();
        services.AddSingleton<IFactory<IFormatter?>, FormatterFactory>();
        services.AddTransient<IAccountsRepository, AccountsRepository>();
        services.AddTransient<IAccountsService, AccountsService>();
        services.AddSwaggerGen(opt =>
        {
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, 
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleBankAPI", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidAudience = configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"]))
                };
            }
        );
    }
}