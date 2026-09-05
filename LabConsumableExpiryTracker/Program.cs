using home.mahindra.RiderProjects.LatihanEFCore.LatihanEFCore.Data;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.Mappings;
using LabConsumableExpiryTracker.Repositories;
using LabConsumableExpiryTracker.Services;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ILotRepository, LotRepository>();

builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<ILotService, LotServices>();

builder.Services.AddScoped<IDbinitializer, DbInitializer>();
builder.Services.AddAutoMapper(typeof(LotMappingProfile));


builder.Services.AddAutoMapper(typeof(LotMappingProfile));

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDbinitializer>();
    await initializer.Initialized();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
