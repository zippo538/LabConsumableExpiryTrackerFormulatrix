using System.Text;
using FluentValidation;
using home.mahindra.RiderProjects.LatihanEFCore.LatihanEFCore.Data;
using LabConsumableExpiryTracker.Configurations;
using LabConsumableExpiryTracker.Data.Seeders;
using LabConsumableExpiryTracker.Mapping;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Repositories;
using LabConsumableExpiryTracker.Services;
using LabConsumableExpiryTracker.Services.Interfaces;
using LabConsumableExpiryTracker.Validators.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Configuration
// ==================================================

var connectionString = builder.Configuration.GetConnectionString(
    "DefaultConnection"
);

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings is not configured."
    );


// ==================================================
// Database
// ==================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);


// ==================================================
// Identity
// ==================================================

builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();


// ==================================================
// Authentication & Authorization
// ==================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)
            )
        };
    });

builder.Services.AddAuthorization();


// ==================================================
// Application Services
// ==================================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddSingleton<ILotRepository, LotRepository>();

builder.Services.AddScoped<IDbinitializer, DbInitializer>();


// ==================================================
// Fluent Validation
// ==================================================

builder.Services.AddValidatorsFromAssemblyContaining<CreateScientistRequestValidator>();
builder.Services.AddFluentValidationAutoValidation(); // dari package MVC, otomatis global ke semua controller


// ==================================================
// AutoMapper
// ==================================================

builder.Services.AddAutoMapper(typeof(UserMappingProfile));


// ==================================================
// Controllers
// ==================================================

builder.Services.AddControllers();


// ==================================================
// Swagger
// ==================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description =
            "Masukkan JWT token. Contoh: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// ==================================================
// Build Application
// ==================================================

var app = builder.Build();


// ==================================================
// Database Initialization / Seeder
// ==================================================

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<IDbinitializer>();

    await initializer.Initialized();
}


// ==================================================
// HTTP Request Pipeline
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();