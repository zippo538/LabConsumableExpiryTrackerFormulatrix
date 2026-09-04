using home.mahindra.RiderProjects.LatihanEFCore.LatihanEFCore.Data;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<ILotRepository, LotRepository>();

builder.Services.AddScoped<IDbinitializer, DbInitializer>();

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
