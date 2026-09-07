using FluentValidation;
using LabConsumableExpiryTracker.Data;
using LabConsumableExpiryTracker.Data.Seeders;
using LabConsumableExpiryTracker.DTOs;
using LabConsumableExpiryTracker.Mappings;
using LabConsumableExpiryTracker.Repositories;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using LabConsumableExpiryTracker.Services;
using LabConsumableExpiryTracker.Services.Interfaces;
using LabConsumableExpiryTracker.Validators;
using LabConsumableExpiryTracker.Validators.ItemValidator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<ILotService, LotService>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<IValidator<CreateItemDto>, CreateItemValidator>();
builder.Services.AddScoped<IValidator<UpdateItemDto>, UpdateItemValidator>();

builder.Services.AddScoped<IDbinitializer, DbInitializer>();
builder.Services.AddScoped<ItemLotSeeder>();

builder.Services.AddAutoMapper(
    typeof(LotMappingProfile),
    typeof(ItemMappingProfile));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer =
        scope.ServiceProvider.GetRequiredService<IDbinitializer>();

    await initializer.Initialized();

    var itemLotSeeder =
        scope.ServiceProvider.GetRequiredService<ItemLotSeeder>();

    await itemLotSeeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();