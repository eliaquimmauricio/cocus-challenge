using Cocus.Domain.Interfaces.Services;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Domain.Services;
using Cocus.Infra.Data.Context;
using Cocus.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IFlightService, FlightService>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
	options.LoginPath = "/Login/Index";
	options.LogoutPath = "/Login/Logout";
	options.AccessDeniedPath = "/Login/Index";
})
.AddGoogle(options =>
{
	options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
	options.CallbackPath = "/signin-google";
	options.SaveTokens = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Index}/{id?}")
	.WithStaticAssets();


app.Run();
