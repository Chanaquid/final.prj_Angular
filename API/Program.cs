using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using StackExchange.Redis;
using Infrastructure.Factories;
using API.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Core.Entities.Identity;
using Infrastructure.Services;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy => {
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .WithOrigins("https://localhost:4200")
          .WithExposedHeaders("Access-Control-Allow-Origin");
});

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentityServices(builder.Configuration);

//Connection string
builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Registering the ProductRepository with the DI system
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductBrandRepository, ProductBrandRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// Fetching the Redis connection string and setting up Redis
var redisConfig = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<RedisConnectionFactory>(new RedisConnectionFactory(redisConfig));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => sp.GetRequiredService<RedisConnectionFactory>().Connection());


//Configure autoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");


app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<StoreContext>();
var IdentityContext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();

var logger = services.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();
    await IdentityContext.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUsersAsyn(userManager);
}
catch (Exception ex) 
{
    
    logger.LogError(ex, "An error occured during migration");
}




app.Run();