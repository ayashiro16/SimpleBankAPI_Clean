using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.API.ExceptionHandlers;
using SimpleBankAPI.Infrastructure.Data;
using SimpleBankAPI.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var dbContext = services.GetRequiredService<AccountContext>();
    if (dbContext.Database.IsSqlServer())
    {
        dbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    throw;
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.AddErrorHandler();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();