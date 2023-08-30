using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using StockMarketSolution;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllersWithViews();
builder.Services.Configure<TradingOptions>(builder.Configuration.GetSection("TradingOptions"));
builder.Services.AddScoped<IStocksService, StocksService>();
builder.Services.AddScoped<IFinnhubService, FinnhubService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();


