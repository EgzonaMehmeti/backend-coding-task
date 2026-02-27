using Claims.Auditing;
using Claims.Data;
using Claims.Repositories;
using Claims.Repositories.Interfaces;
using Claims.Services;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddDbContext<ClaimsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ICoverRepository, CoverRepository>();

builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();

builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddHostedService<AuditBackgroundService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
    context.Database.Migrate();

    var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();
    auditContext.Database.Migrate();
}

app.Run();

public partial class Program { }
